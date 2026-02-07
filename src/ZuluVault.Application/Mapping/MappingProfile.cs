using AutoMapper;
using ZuluVault.Application.DTOs;
using ZuluVault.Application.Features.Auth.Commands;
using ZuluVault.Domain.Entities;
using ZuluVault.Domain.Entities.Wallet;

namespace ZuluVault.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Wallet mappings
        CreateMap<Wallet, WalletDto>();
        CreateMap<Wallet, WalletSummaryDto>();
        CreateMap<CreateWalletDto, Wallet>();

        // Transaction mappings
        CreateMap<Transaction, TransactionDto>();
        CreateMap<Transaction, TransactionHistoryDto>();

        // User mappings
        CreateMap<User, UserDto>();
        CreateMap<RegisterCommand, User>();

        // Audit mappings
        CreateMap<AuditLog, AuditLog>();
    }
}
