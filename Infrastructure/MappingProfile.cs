/// <summary>
/// AutoMapper
/// </summary>
public class MappingProfile : Profile {
    public MappingProfile() {
        #region SqlServer
        CreateMap<HoSoCongViecCreateRequest, HoSoCongViec>()
            .ForMember(
            des => des.CreatedUserId, // map thành trường nào
            opt => opt.MapFrom(act => act.UserId)); // map từ trường nào
        CreateMap<HoSoCongViecUpdateRequest, HoSoCongViec>()
            .ForMember(
            des => des.UpdatedUserId,
            opt => opt.MapFrom(act => act.UserId));
        CreateMap<HoSoCongViec, HoSoCongViecResponse>()
            .ForMember(des => des.NgayMo, opt => opt.MapFrom(act => act.CreatedDate));

        #endregion SqlServer
    }
}


# MappingTypes:
//  map a collection of objects to a collection of objects:
var result = _mapper.Map<IEnumerable<HoSoCongViec>, IEnumerable<HoSoCongViecResponse>>(entitys);

// map a single object to a single object
var entity = _mapper.Map<HoSoCongViecCreateRequest, HoSoCongViec>(request);

// maps the data from object to the entity object
// used when already have an existing destination object and want to update its properties
_mapper.Map(request, entity);






