# MappingTypes:
//  map a collection of objects to a collection of objects:
var result = _mapper.Map<IEnumerable<HoSoCongViec>, IEnumerable<HoSoCongViecResponse>>(entitys);

// map a single object to a single object
var entity = _mapper.Map<HoSoCongViecCreateRequest, HoSoCongViec>(request);

// maps the data from object to the entity object
// used when already have an existing destination object and want to update its properties
_mapper.Map(request, entity);


# MappingProfile
public class MappingProfile : Profile {
    public MappingProfile() {
        #region SqlServer
        CreateMap<HoSoCongViecCreateRequest, HoSoCongViec>()
            .ForMember(
                des => des.CreatedUserId, // map thành trường nào
                opt => opt.MapFrom(src => src.UserId) // map từ trường nào
            )
            .ForMember(
                dest => dest.Name, 
                opt => opt.Ignore() 
            ) // tức là field "Name" của "HoSoCongViec" sẽ không thay đổi sau mapping
            .ForMember(
                dest => dest.FirstName,  
                opt => opt.MapFrom(src => "Poppy") 
            ) // tức là value của field "FirstName" của "HoSoCongViec" sẽ "Poppy" luôn là sau mapping
            .ForMember(
                dest => dest.LastName,  
                opt => opt.MapFrom(/*variable*/) 
            ); // use "variable" as the value for the destination property "LastName"

        CreateMap<HoSoCongViec, HoSoCongViecResponse>()
            .ForMember(des => des.NgayMo, opt => opt.MapFrom(act => act.CreatedDate));

        #endregion SqlServer
    }
}

# CustomValueResolver
// -> implements the "IValueResolver<TSource, TDestination, TDestMember>"
// -> provides custom logic:
// => for mapping a "source value" to a "destination value"
// -> used when default mapping not enough, need to perform more complex mapping operations

# CustomTypeConverter
// -> implements the "ITypeConverter<TSource, TDestination>"
// -> provides custom logic:
// => for converting a "source object of one type" to a "destination object of another type"
// -> used when perform complex type conversions that default mapping behavior provided by AutoMapper not enough







