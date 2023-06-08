using MediatR;
using MapsterMapper;
using Domain.Common;

namespace Application.Features.Players.Queries;
public record GetPlayersQuery : IRequest<IEnumerable<PlayerResponse>>;

public class GetPlayersQueryHandler : IRequestHandler<GetPlayersQuery, IEnumerable<PlayerResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPlayersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PlayerResponse>> Handle(GetPlayersQuery request, CancellationToken cancellationToken)
    {
        var players = await _unitOfWork.Players.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<PlayerResponse>>(players);
    }
}