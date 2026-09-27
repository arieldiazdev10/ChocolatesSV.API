using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL.Interfaces
{
    public interface IContentService
    {
        Task<AboutDto> GetAboutAsync();

        Task<List<FaqDto>> GetFaqAsync();
    }
}