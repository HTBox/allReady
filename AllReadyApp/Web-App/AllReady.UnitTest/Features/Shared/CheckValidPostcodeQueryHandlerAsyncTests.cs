using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Features.Shared;
using AllReady.Models;
using Xunit;

namespace AllReady.UnitTest.Features.Shared
{
	public class CheckValidPostcodeQueryHandlerAsyncTests : InMemoryContextTest
	{
		private List<PostalCodeGeo> _postalCodeGeos;

		[Fact]
		public async Task ValidatesToTrueForValidStoredCodes()
		{
			//arrange
			var handler = new CheckValidPostcodeQueryHandler( Context );
			var message = new CheckValidPostcodeQuery()
			{
				Postcode = new PostalCodeGeo
				{
					City = _postalCodeGeos[0].City,
					PostalCode = _postalCodeGeos[0].PostalCode,
					State = _postalCodeGeos[0].State
				}
			};

			//act
			var result = await handler.Handle( message );

			//assert
			Assert.True( result );
		}

		[Theory]
		[InlineData( true, true, false )]
		[InlineData( true, false, false )]
		[InlineData( false, false, false )]
		[InlineData( false, true, false )]
		[InlineData( false, true, true )]
		[InlineData( false, false, true )]
		public async Task ReturnsFalseIfCityPostalCodeStateCombinationDoesNotMatch( bool cityFlag,
																					bool postalcodeFlag,
																					bool stateFlag )
		{
			//arrange
			var handler = new CheckValidPostcodeQueryHandler( Context );
			var message = new CheckValidPostcodeQuery
			{
				Postcode = new PostalCodeGeo
				{
					City = cityFlag ? _postalCodeGeos[1].City : "test",
					PostalCode = postalcodeFlag ? _postalCodeGeos[1].PostalCode : "test",
					State = stateFlag ? _postalCodeGeos[1].State : "test"
				}
			};

			//act
			var result = await handler.Handle( message );

			//assert
			Assert.False( result );
		}

		protected override void LoadTestData()
		{
			var randomGenerator = new Random();

			_postalCodeGeos = new List<PostalCodeGeo>();
			Enumerable.Range( 0, 5 ).ToList().ForEach( x => _postalCodeGeos.Add( new PostalCodeGeo()
			{
				City = $"test city{randomGenerator.Next( 1000 )}",
				PostalCode = $"{randomGenerator.Next( 10000 ):00000}",
				State = $"{randomGenerator.Next( 99 ):00}"
			} ) );

			Context.PostalCodes.AddRange( _postalCodeGeos );
			Context.SaveChanges();
		}
	}
}
