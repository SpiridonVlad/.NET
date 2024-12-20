﻿using Application.Use_Cases.Commands;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;


namespace Application.Use_Cases.CommandHandlers
{
    public class CreateListingCommandHandler(IListingRepository repository, IMapper mapper, IUserRepository userRepository, IPropertyRepository propertyRepository) : IRequestHandler<CreateListingCommand, Result<Guid>>
    {
        private readonly IListingRepository repository = repository;
        private readonly IUserRepository userRepository = userRepository; 
        private readonly IPropertyRepository propertyRepository = propertyRepository;
        private readonly IMapper mapper = mapper;

        public async Task<Result<Guid>> Handle(CreateListingCommand request, CancellationToken cancellationToken)
        {
            var userId = await ValidateUSerId(request.UserId);
            var propertyId = await ValidatePropertyId(request.PropertyId);
            if (!userId.IsSuccess)
            {
                return Result<Guid>.Failure(userId.ErrorMessage);
            }
            if (!propertyId.IsSuccess)
            {
                return Result<Guid>.Failure(propertyId.ErrorMessage);
            }
            var listing = mapper.Map<Listing>(request);
            var result = await repository.AddAsync(listing);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
        public async Task<Result<Guid>> ValidateUSerId(Guid userId)
        {
            var result = await userRepository.GetByIdAsync(userId);
            if (result == null)
            {
                return Result<Guid>.Failure("User not found");
            }
            return Result<Guid>.Success(userId);
        }
        public async Task<Result<Guid>> ValidatePropertyId(Guid propertyId)
        {
            var result = await propertyRepository.GetByIdAsync(propertyId);
            if (result == null)
            {
                return Result<Guid>.Failure("Property not found");
            }
            return Result<Guid>.Success(propertyId);
        }
    }
}
