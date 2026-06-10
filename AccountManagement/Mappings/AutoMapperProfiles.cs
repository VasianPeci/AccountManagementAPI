using AccountManagement.DTO;
using AccountManagement.Models.Domain;
using AutoMapper;

namespace AccountManagement.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<BankAccount, AccountReportDto>()
                .ForMember(dest => dest.ClientCode,
                    opt => opt.MapFrom(src => src.Client.UserId))
                .ForMember(dest => dest.ClientName,
                    opt => opt.MapFrom(src => src.Client.FirstName + " " + src.Client.LastName))
                .ForMember(dest => dest.AccountCode,
                    opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.AccountName,
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Currency,
                    opt => opt.MapFrom(src => src.CurrencyId))
                .ForMember(dest => dest.CurrentBalance,
                    opt => opt.MapFrom(src => src.Balance));

            CreateMap<BankTransaction, TransactionReportDto>()
                .ForMember(dest => dest.Action,
                    opt => opt.MapFrom(src => src.Action == 0 ? "Depozitim" : "Terheqje"))
                .ForMember(dest => dest.Amount,
                    opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(src => src.DateCreated));

            CreateMap<BankAccount, ClientAccountReportDto>()
                .ForMember(dest => dest.AccountId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AccountCode,
                    opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.AccountName,
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Currency,
                    opt => opt.MapFrom(src => src.CurrencyId))
                .ForMember(dest => dest.CurrentBalance,
                    opt => opt.MapFrom(src => src.Balance));

            CreateMap<Currency, CurrencyDto>().ReverseMap();
            CreateMap<Currency, AddCurrencyDto>().ReverseMap();
            CreateMap<Currency, UpdateCurrencyDto>().ReverseMap();

            CreateMap<Client, ClientDto>()
                .ForMember(dest => dest.Username,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Roles,
                    opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.UserId,
                    opt => opt.Ignore());

            CreateMap<BankTransaction, BankTransactionDto>().ReverseMap();
            CreateMap<BankTransaction, AddBankTransactionDto>().ReverseMap();
            CreateMap<BankTransaction, UpdateBankTransactionDto>().ReverseMap();
            CreateMap<BankAccount, BankAccountDto>().ReverseMap();
            CreateMap<BankAccount, AddBankAccountDto>().ReverseMap();
            CreateMap<BankAccount, UpdateBankAccountDto>().ReverseMap();
        }
    }
}
