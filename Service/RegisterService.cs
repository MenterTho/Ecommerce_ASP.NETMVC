using AutoMapper;
using Ecommerce.Data;

namespace Ecommerce.Service
{
	public class RegisterService
	{
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;
        public RegisterService(Hshop2023Context context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }
        

    }

}
