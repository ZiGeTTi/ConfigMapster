using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;



public class MongoRepository<TDocument>:IMongoRepository<TDocument> where TDocument : IDocument
{

    private readonly IMongoCollection<TDocument> _collection;
    private readonly ILogger<MongoRepository<TDocument>> _logger;
    public MongoRepository(IMongoDbSettings mongoDbSettings, ILogger<MongoRepository<TDocument>> logger)
    {
        _logger = logger;
        var database = new MongoClient(mongoDbSettings.ConnectionString).GetDatabase(mongoDbSettings.DatabaseName);
        _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));

    }
    private protected string GetCollectionName(Type documentType)
    {
        return ((BsonCollectionAttribute) documentType.GetCustomAttributes(
                typeof(BsonCollectionAttribute),
                true)
            .FirstOrDefault())?.CollectionName;
    }
    
    public virtual IQueryable<TDocument> AsQueryable()
    {
        return _collection.AsQueryable();
    }

    public async Task<List<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken)
    {
        var result = await _collection.FindAsync(filterExpression, cancellationToken:cancellationToken);

        return result.ToEnumerable().ToList();
    }

    public IEnumerable<TProjected> FilterBy<TProjected>(Expression<Func<TDocument, bool>> filterExpression, Expression<Func<TDocument, TProjected>> projectionExpression)
    {
        return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
    }

    public TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
    {
        return _collection.Find(filterExpression).FirstOrDefault();
    }

    public virtual async Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return await _collection.Find(filterExpression).FirstOrDefaultAsync();
    }

    public TDocument FindById(Guid id)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
        return _collection.Find(filter).SingleOrDefault();
    }

    public Task<TDocument> FindByIdAsync(Guid id)
    {
        return Task.Run(() =>
        {
            //var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
            return _collection.Find(filter).SingleOrDefaultAsync();
        });
    }

    public void InsertOne(TDocument document)
    {
        _collection.InsertOne(document);
    }

    public async Task InsertOneAsync(TDocument document)
    {
        try
        {
          await _collection.InsertOneAsync(document);
        }
        catch (Exception ex)
        {
            CustomLoggerPersistence<MongoRepository<TDocument>>.LogError(_logger,ex,ex.Message,"MONGO-REPO-04");
            throw new MongoRepoException();
        }
      
    }


    public virtual async Task InsertManyAsync(ICollection<TDocument> documents)
    {
        try
        {
            await _collection.InsertManyAsync(documents);
        }
        catch (Exception ex)
        {
            CustomLoggerPersistence<MongoRepository<TDocument>>.LogError(_logger,ex,ex.Message,"MONGO-REPO-03");
            throw new MongoRepoException();
        }
       
    }

    public void ReplaceOne(TDocument document)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
        _collection.FindOneAndReplace(filter, document);
    }

    public virtual async Task ReplaceOneAsync(TDocument document, CancellationToken cancellationToken)
    {
        try
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            var doc =  await _collection.FindOneAndReplaceAsync(filter, document,cancellationToken:cancellationToken);
        }
        catch (Exception ex)
        {
            CustomLoggerPersistence<MongoRepository<TDocument>>.LogError(_logger,ex,ex.Message,"MONGO-REPO-04");
            throw new MongoRepoException();
        }
    
    }

    public void DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
    {
        _collection.FindOneAndDelete(filterExpression);
    }

    public Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return Task.Run(() => _collection.FindOneAndDeleteAsync(filterExpression));
    }

    public void DeleteById(Guid id)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
        _collection.FindOneAndDelete(filter);
    }

    public Task DeleteByIdAsync(Guid id)
    {
        return Task.Run(() =>
        {
            //var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
            _collection.FindOneAndDeleteAsync(filter);
        });
    }

    public void DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
    {
        _collection.DeleteMany(filterExpression);
    }

    public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return Task.Run(() => _collection.DeleteManyAsync(filterExpression));;
    }

    public async Task UpdateOneAsync(TDocument document)
    {
      await  _collection.UpdateOneAsync(
            Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id),
            Builders<TDocument>.Update
                .Set(doc => doc.LastModifiedAt, DateTime.UtcNow)
                .Set(doc => doc, document)
        );
    }
}