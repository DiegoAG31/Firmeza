using AutoMapper;
using Firmeza.Application.DTOs;
using Firmeza.Domain.Entities;

namespace Firmeza.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product Mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // Customer Mappings
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerDto, Customer>();
        CreateMap<UpdateCustomerDto, Customer>();

        // Sale Mappings
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty));
        CreateMap<SaleDetail, SaleDetailDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
            .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.Product != null ? src.Product.Code : string.Empty));
        
        CreateMap<CreateSaleDto, Sale>();
        CreateMap<CreateSaleDetailDto, SaleDetail>();
    }
}
