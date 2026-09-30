using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL.Interfaces
{
    public interface IPaymentSimulationService
    {
        Task<SimulatePaymentResponseDto> SimulateAsync(SimulatePaymentRequestDto request);
    }
}
