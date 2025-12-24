using Domain.Models.PeriodLock;

namespace Application;

public class PeriodLockMappingProfile : Profile
{
    public PeriodLockMappingProfile()
    {
        CreateMap<CreatePeriodLockModel, PeriodLockEntity>()
            .ForMember(dest => dest.Shift, opt => opt.MapFrom(src => Enum.Parse<ShiftType>(src.Shift!)))
            .ReverseMap();
        CreateMap<ClosePeriodLockModel, PeriodLockEntity>().ReverseMap();
        CreateMap<PeriodLockNoteModel, PeriodLockNoteEntity>().ReverseMap();
        CreateMap<InventoryPeriodLockModel, InventoryPeriodLockEntity>().ReverseMap();

        // ⭐⭐ THÊM 2 MAP lấy lịch sử chốt ca ⭐⭐
        CreateMap<PeriodLockEntity, PeriodLockWithNotesDto>()
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Note));

        CreateMap<PeriodLockNoteEntity, PeriodLockNoteDto>();
    }
}
