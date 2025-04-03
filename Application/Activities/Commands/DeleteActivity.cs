using System;
using Application.Core;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class DeleteActivity
{
  public class Command : IRequest<Result<Unit>>
  {
    public required string Id {get;set;}
  }
    public class Handler(AppDbContext context) : IRequestHandler<Command ,Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var activityFromDb = await context.Activities
                        .FindAsync([request.Id] , cancellationToken)
                                ?? throw new Exception("Cannot find Activity");
            if(activityFromDb == null) return Result<Unit>.Failure(error: "Activity not found", code: 404);  
                              
            context.Remove(activityFromDb);
            var result = await context.SaveChangesAsync(cancellationToken)> 0;  
            if(!result)  Result<Unit>.Failure(error: "Failed to delete activity", code: 400);
            
             return Result<Unit>.Success(value: Unit.Value);    


        }
    }
}
