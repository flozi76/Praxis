using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Duftfinder.Domain.Dtos;
using Duftfinder.Domain.Entities;
using Duftfinder.Domain.Enums;
using Duftfinder.Domain.Helpers;

namespace Duftfinder.Web.Models
{
    /// <summary>
    /// Represents the model for configurations that is used for the view.
    /// </summary>
    /// <author>Anna Krebs</author>
    public class ConfigurationViewModel
    {
        private readonly Configuration _configuration;


        public ConfigurationViewModel()
        {
            _configuration = new Configuration();
        }

        public ConfigurationViewModel(Configuration configuration)
        {
            if (configuration == null)
            {
                _configuration = new Configuration();
            }
            else
            {
                _configuration = configuration;
            }
        }

        public string Id
        {
            get { return _configuration.Id; }
            set { _configuration.Id = value; }
        }

        public string Key
        {
            get { return _configuration.Key; }
            set { _configuration.Key = value; }
        }

        public string Value
        {
            get { return _configuration.Value; }
            set { _configuration.Value = value; }
        }

        public string Description
        {
            get { return _configuration.Description; }
            set { _configuration.Description = value; }
        }

        /// <summary>
        /// Map values from View to Entity.
        /// </summary>
        /// <author>Anna Krebs</author>
        /// <param name="configuration"></param>
        public void MapViewModelToEntity(Configuration configuration)
        {
            configuration.Id = Id;
            configuration.Key = Key;
            configuration.Value = Value;
            configuration.Description = Description;
        }
    }

}

