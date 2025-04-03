using Application.Activities.DTOs;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Commands;

public class EditActivity
{
public class Command : IRequest<Result<Unit>>
{
  public required EditActivityDto ActivityDto {get; set;}
}
    public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var activityFromDb = await context.Activities
                        .FindAsync([request.ActivityDto.Id] , cancellationToken)
                                ?? throw new Exception("Cannot find Activity");

            activityFromDb = mapper.Map(request.ActivityDto, activityFromDb); // если не присваивать значение, объект назначения не обновляется
             if(activityFromDb == null) return Result<Unit>.Failure(error: "Activity not found", code: 404);
            //Console.WriteLine(request.Activity.Title);
            //Console.WriteLine(activityFromDb.Title);
            //activityFromDb.Description = request.Activity.Description + "_1";
            
            context.ChangeTracker.Clear(); // нужно, чтобы не было исключений о дубле трэкинга

            context.Entry(activityFromDb).State = EntityState.Modified; // после map activityFromDb не маркируется в трэкинге, как измененная
            
            var result = await context.SaveChangesAsync(cancellationToken)> 0;

            if(!result)  Result<Unit>.Failure(error: "Failed to save activity", code: 400);
            return Result<Unit>.Success(value: Unit.Value); 
        }
    }
}
