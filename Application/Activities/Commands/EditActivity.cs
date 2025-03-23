using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Commands;

public class EditActivity
{
public class Command : IRequest
{
  public required Activity Activity {get; set;}
}
    public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Command>
    {
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var activityFromDb = await context.Activities
                        .FindAsync([request.Activity.Id] , cancellationToken)
                                ?? throw new Exception("Cannot find Activity");

            activityFromDb = mapper.Map(request.Activity, activityFromDb); // если не присваивать значение, объект назначения не обновляется
            
            //Console.WriteLine(request.Activity.Title);
            //Console.WriteLine(activityFromDb.Title);
            //activityFromDb.Description = request.Activity.Description + "_1";
            
            context.ChangeTracker.Clear(); // нужно, чтобы не было исключений о дубле трэкинга

            context.Entry(activityFromDb).State = EntityState.Modified; // после map activityFromDb не маркируется в трэкинге, как измененная
            
            await context.SaveChangesAsync(cancellationToken);
           
        }
    }
}
