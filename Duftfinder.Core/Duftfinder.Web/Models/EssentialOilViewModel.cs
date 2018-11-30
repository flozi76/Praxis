using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Web.Models
{
    /// <summary>
    /// Represents the model for "Ätherische Öle" that is used for the view.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class EssentialOilViewModel
    {
        private readonly EssentialOil _essentialOil;

        private readonly EssentialOilType? _essentialOilType;

        public EssentialOilViewModel()
        {
            _essentialOil = new EssentialOil();
        }

        public EssentialOilViewModel(EssentialOil essentialOil)
        {
            _essentialOil = essentialOil;

            if (_essentialOil.Type != null)
            {
                // Parses enum to string in order to display the appropriate name of the essential oil type.
                EssentialOilType e;
                _essentialOilType = Enum.TryParse(_essentialOil.Type, true, out e) ? (EssentialOilType?)e : null;
            }
        }

        public string Id
        {
            get { return _essentialOil.Id; }
            set { _essentialOil.Id = value; }
        }

        [Required(ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "Error_InputRequired")]
        public string Name
        {
            get { return _essentialOil.Name; }
            set { _essentialOil.Name = value; }
        }

        /// <summary>
        /// The first letter of the essential oil name.
        /// </summary>
        /// <author>Anna Krebs</author>
        public string FirstLetterOfEssentialOilName
        {
            get { return Name.Substring(0, 1).ToUpper(); }
        }

        public string NameLatin
        {
            get { return _essentialOil.NameLatin; }
            set { _essentialOil.NameLatin = value; }
        }

        public string Description
        {
            get { return _essentialOil.Description; }
            set { _essentialOil.Description = value; }
        }

        public string Type
        {
            get
            {
                // Set "Oil" by default for radio button.
                if (string.IsNullOrEmpty(_essentialOil.Type))
                {
                    return EssentialOilType.Oil.ToString();
                }
                return _essentialOil.Type;
            }
            set { _essentialOil.Type = value; }
        }

        public string PictureFileName
        {
            get { return _essentialOil.PictureFileName; }
            set { _essentialOil.PictureFileName = value; }
        }

        public string PictureDataAsString
        {
            get { return _essentialOil.PictureDataAsString; }
            set { _essentialOil.PictureDataAsString = value; }
        }

        /// <summary>
        /// Makes base 64 picture string displayable for view.
        /// </summary>
        /// <author>Anna Krebs</author>
        public string PictureDataAsStringDisplay
        {
            get
            {
                // Only display picture if value is set.
                if (!string.IsNullOrEmpty(_essentialOil.PictureDataAsString))
                {
                    return $"data:image/gif;base64,{_essentialOil.PictureDataAsString}";
                }

                return null;
            }
        }

        public string PictureSource
        {
            get { return _essentialOil.PictureSource; }
            set { _essentialOil.PictureSource = value; }
        }

        public IList<Effect> Effects
        {
            get { return _essentialOil.Effects; }
            set { _essentialOil.Effects = value; }
        }   
             
        public IList<Molecule> Molecules
        {
            get { return _essentialOil.Molecules; }
            set { _essentialOil.Molecules = value; }
        }

        /// <summary>
        /// Displays name of essential oil type enum.
        /// </summary>
        /// <author>Anna Krebs</author>
        public string EssentialOilTypeDisplayName
        {
            get { return _essentialOilType?.Display(); }
        }

        // Search stuff

        /// <summary>
        /// The "Anzahl Übereinstimmungen" of the effect search.
        /// </summary>
        /// <author>Anna Krebs</author>
        public int MatchAmount { get; set; }

        /// <summary>
        /// The product of EffectDegree * DiscomfortValue.
        /// (Wirksamkeit * Beschwerdeausmass)
        /// </summary>
        /// <author>Anna Krebs</author>
        public int EffectDegreeDiscomfortValue { get; set; }

        /// <summary>
        /// The "Gewichtete Übereinstimmung"
        /// The EffectDegreeDiscomfortValue in relation to the maxEffectDegreeDiscomfortValue
        /// of all the essential oils in the search result list.
        /// </summary>
        public int WeightedMatchValue { get; set; }

        /// <summary>
        /// The list with the searched effects.
        /// </summary>
        /// <author>Anna Krebs</author>
        public IList<SearchEffectItem> SearchEffects { get; set; }

        /// <summary>
        /// All the effects, that an essential oil is effective for as one string separated with ;@
        /// </summary>
        /// <author>Anna Krebs</author>
        public string SearchEffectTextsInEssentialOil { get; set; }

        /// <summary>
        /// The text of the searched essential oil.
        /// </summary>
        /// <author>Anna Krebs</author>
        public string SearchEssentialOilText { get; set; }

        /// <summary>
        /// Map values from View to Entity.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="essentialOil"></param>
        public void MapViewModelToEntity(EssentialOil essentialOil)
        {
            essentialOil.Id = Id;
            essentialOil.Name = Name;
            essentialOil.NameLatin = NameLatin;
            essentialOil.Description = Description;
            essentialOil.Type = Type;
            essentialOil.PictureSource = PictureSource;
            essentialOil.Effects = Effects;
            essentialOil.Molecules = Molecules;
            
            SetPictureValues(essentialOil);
        }

        /// <summary>
        /// Sets picture according to whether is new upload, picture is removed, picture is the same etc.
        /// </summary>
        /// <param name="essentialOil"></param>
        private void SetPictureValues(EssentialOil essentialOil)
        {
            // essentialOil.PictureDataAsString -> current picture.
            // PictureDataAsString -> new uploaded picture.

            bool isSamePicture = PictureDataAsString != null && essentialOil.PictureDataAsString == null;
            bool isSamePictureFileName = PictureFileName != null && essentialOil.PictureFileName == null;

            // Keep already uploaded picture if edit & picture wasn't removed.
            if (isSamePicture)
            {
                essentialOil.PictureDataAsString = PictureDataAsString;
            }

            // Keep already uploaded picture file name if edit & picture wasn't removed.
            if (isSamePictureFileName)
            {
                essentialOil.PictureFileName = PictureFileName;
            }

            // Remove PictureSource text if no picture is set.
            bool noPictureSet = PictureDataAsString == null && essentialOil.PictureDataAsString == null;
            if (noPictureSet)
            {
                essentialOil.PictureSource = null;
            }
        }
    }

}

