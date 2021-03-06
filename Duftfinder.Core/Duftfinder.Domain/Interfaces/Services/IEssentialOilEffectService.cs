﻿using System.Threading.Tasks;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Filters;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Domain.Interfaces.Services
{
	/// <summary>
	///     Represents the interface for the business logic service of "Wirkungen" for "Ätherisches Öl".
	/// </summary>
	/// <author>Anna Krebs</author>
	public interface IEssentialOilEffectService : IService<EssentialOilEffect, EssentialOilEffectFilter>
	{
		Task<ValidationResultList> DeleteAssignedEffectsAsync(string essentialOilId);

		Task<ValidationResultList> DeleteAssignedEssentialOilsAsync(string effectId);
	}
}