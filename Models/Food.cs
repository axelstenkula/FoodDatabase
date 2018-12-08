using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// using FoodDatabase.ResultExtensions;

namespace FoodDatabase
{
    public class FoodContext : DbContext
    {
        public FoodContext(DbContextOptions<FoodContext> options)
            : base(options)
        { }

        public DbSet<Food> Foods { get; set; }
    }

    // Model
    public class Food
    {
        [Key]
        [Column("id")]
        public Guid Id {get; set; }

    }

    public class FoodDto
    {
        public Guid Id { get; set; }
        internal static FoodDto FromDomainModel(Food model)
        {
            return new FoodDto(){
                Id = model.Id
            };
        }
    }

    public class FoodViewModel : FoodDto
    {
        internal FoodDto ToDto()
        {
            return new FoodDto() {
                Id = this.Id
            };
        }
    }

    public interface IFoodRepository 
    {
        Task<IEnumerable<Food>> GetAll();
        Task<Food> CreateAsync(Food model);
    }
    public class FoodRepository : IFoodRepository
    {
        protected FoodContext Context;

        public FoodRepository(FoodContext context)
        {
            this.Context = context;
        }

        public async Task<Food> CreateAsync(Food model)
        {
            this.Context.Set<Food>().Add(model);
            await this.Context.SaveChangesAsync();
            return model;
        }

        public async Task<IEnumerable<Food>> GetAll()
        {
            var query = this.Context.Set<Food>().AsQueryable();

            return await query
                            .Take(10)
                            .AsNoTracking()
                            .ToListAsync();

        }

    }

    public interface IFoodService {
        Task<IResult<FoodDto>> CreateAsync(FoodDto viewModel);
        Task<IEnumerable<FoodDto>> GetAllAsync();
    }
  
    public class FoodService : IFoodService
    {
        protected IFoodRepository Repository;

        public FoodService(IFoodRepository repository)
        {
            this.Repository = repository;
        }

        public async Task<IEnumerable<FoodDto>> GetAllAsync()
        {
            var res = await this.Repository.GetAll();

            IEnumerable<FoodDto> dtos = res.Select((model) => this.DomainModelToDto(model));
            return dtos;
        }

        public async Task<IResult<FoodDto>> CreateAsync(FoodDto dto)
        {
            Food model = this.DtoToDomainModel(dto);
            model = await this.Repository.CreateAsync(model);
            return this.DomainModelToResult(model);
        }

        public IResult<FoodDto> DomainModelToResult(Food model)
        {
            if (model == null)
            {
                return ResultExtensions.Error<FoodDto>(ErrorCode.Error, "Not found");
            }

            return ResultExtensions.OK(this.DomainModelToDto(model));
        }

        private Food DtoToDomainModel(FoodDto dto)
        {
            return new Food() {
                Id = dto.Id
            };
        }

        public FoodDto DomainModelToDto(Food model)
        {
            return FoodDto.FromDomainModel(model);
        }



    }

    public interface IResult<T>
    {
        bool IsError();
        IError Error { get; }
        T Value { get; }
    }

    public static class ResultExtensions {
        public static IResult<T> Error<T>(ErrorCode code, string message)
        {
            return new Result<T>(new Error(code, message));
        }

        public static IResult<T> OK<T>(T result)
        {
            return new Result<T>(result);
        }
    }

public class Result<T> : IResult<T>
    {
        private T _value;
        public IError Error { get; }
        public T Value
        {
            get
            {
                if (this.IsError())
                {
                    string message = String.Format("Error is set on the object. Value cannot be returned. Error message: {0}",
                                                   Error.Message);
                    throw new InvalidOperationException(message);
                }
                return _value;
            }
            private set
            {
                _value = value;
            }
        }

        public Result(T value)
        {
            this.Value = value;
        }

        public Result(IError error)
        {
            this.Error = error;
        }

        public Result(ErrorCode code, string message)
        {
            this.Error = new Error(code, message);
        }

        public bool IsError()
        {
            return this.Error != null;
        }
    }
    public class Error : IError
    {
        protected string _message;
        protected ErrorCode _code;
        protected List<IError> _causedBy;

        public string Message
        {
            get
            {
                return this._message;
            }
        }

        public ErrorCode Code
        {
            get
            {
                return this._code;
            }
        }

        public IEnumerable<IError> CausedBy
        {
            get
            {
                return this._causedBy.AsEnumerable();
            }
            protected set
            {

            }
        }

        public Error(string message) : this(ErrorCode.Error, message)
        {
        }

        public Error(ErrorCode code, string message)
        {
            this._message = message;
            this._code = code;
            this._causedBy = new List<IError>();
        }

        public override string ToString()
        {
            return this.Message;
        }
    }
    public interface IError
    {
        string Message { get; }
        ErrorCode Code { get; }
        IEnumerable<IError> CausedBy { get; }
    }

public enum ErrorCode{
    Error
}

}