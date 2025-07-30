using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;



public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    Guid Id { get; set; }

    DateTime CreatedAt { get; }

    DateTime? LastModifiedAt { get; set; }

}

public abstract class Document : IDocument
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
public interface IMongoRepository<TDocument> where TDocument: IDocument
{
    IQueryable<TDocument> AsQueryable();

    Task<List<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken);
    
    IEnumerable<TProjected> FilterBy<TProjected>(
        Expression<Func<TDocument, bool>> filterExpression,
        Expression<Func<TDocument, TProjected>> projectionExpression);
    
    TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression);
    
    Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression);
    
    TDocument FindById(Guid id);

    Task<TDocument> FindByIdAsync(Guid id);

    void InsertOne(TDocument document);
    
    Task InsertOneAsync(TDocument document);

 

    Task InsertManyAsync(ICollection<TDocument> documents);

    void ReplaceOne(TDocument document);

    Task ReplaceOneAsync(TDocument document, CancellationToken cancellationToken);
    
    void DeleteOne(Expression<Func<TDocument, bool>> filterExpression);

    Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression);

    void DeleteById(Guid id);

    Task DeleteByIdAsync(Guid id);

    void DeleteMany(Expression<Func<TDocument, bool>> filterExpression);

    Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression);

    Task UpdateOneAsync(TDocument document);
}