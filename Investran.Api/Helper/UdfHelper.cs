using Investran.Integration;
using Sungard.Investran.Suite.WebServices.Contracts.Common;
using Sungard.Investran.Suite.WebServices.Contracts.Common.Lookups;
using Sungard.Investran.Suite.WebServices.Contracts.Common.Udf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Investran.Api.Helper
{
    public class UdfHelper
    {
        private List<FieldDefinitionDto> Fields
        {
            get
            {
                if (_fields == null)
                    _fields = new List<FieldDefinitionDto>();

                lock (_fields)
                {
                    if (CacheExpireIn < DateTime.Now)
                        GetAllUdfs();
                }

                return _fields;
            }
        }
        private DateTime CacheExpireIn { get; set; }
        private List<FieldDefinitionDto> _fields { get; set; }
        private UdfService UdfService { get; }
        public SdsService SdsService { get; }

        public UdfHelper(UdfService udfService, SdsService sdsService)
        {
            UdfService = udfService;
            SdsService = sdsService;
        }

        private void GetAllUdfs()
        {
            _fields = UdfService.All<FieldDefinitionDto>();
            CacheExpireIn = DateTime.Now.AddHours(1);

        }

        public List<FieldDefinitionDto> GetFields(int entityTypeId)
        {
            var fieldsByEntityType = new List<FieldDefinitionDto>();
            foreach (var field in Fields)
            {
                if (field.FieldDefinitionClasses.FirstOrDefault(f => f.EntityType.Id == entityTypeId) != null)
                    fieldsByEntityType.Add(field);
            }

            return fieldsByEntityType;
        }

        public UdfValueDto GetUdf(FieldDefinitionDto fieldDefinition, object udfValue, int entityTypeId)
        {
            switch (fieldDefinition.Type)
            {
                case FieldTypeDto.Boolean:
                    return new BooleanUdfValueDto
                    {
                        EffectiveDate = DateTime.Now,
                        FieldDefinitionClass = fieldDefinition.FieldDefinitionClasses.FirstOrDefault(fdc => fdc.EntityType.Id == entityTypeId),
                        Value = Convert.ToBoolean(udfValue)
                    };

                case FieldTypeDto.Currency:
                    return new CurrencyUdfValueDto
                    {
                        EffectiveDate = DateTime.Now,
                        FieldDefinitionClass = fieldDefinition.FieldDefinitionClasses.FirstOrDefault(fdc => fdc.EntityType.Id == entityTypeId),
                        Value = Convert.ToDecimal(udfValue)
                    };

                case FieldTypeDto.Percentage:
                    return new PercentageUdfValueDto
                    {
                        EffectiveDate = DateTime.Now,
                        FieldDefinitionClass = fieldDefinition.FieldDefinitionClasses.FirstOrDefault(fdc => fdc.EntityType.Id == entityTypeId),
                        Value = Convert.ToDecimal(udfValue)
                    };

                case FieldTypeDto.Numeric:
                    return new NumericUdfValueDto
                    {
                        EffectiveDate = DateTime.Now,
                        FieldDefinitionClass = fieldDefinition.FieldDefinitionClasses.FirstOrDefault(fdc => fdc.EntityType.Id == entityTypeId),
                        Value = Convert.ToDecimal(udfValue)
                    };

                case FieldTypeDto.Date:
                    return new DateUdfValueDto
                    {
                        EffectiveDate = DateTime.Now,
                        FieldDefinitionClass = fieldDefinition.FieldDefinitionClasses.FirstOrDefault(fdc => fdc.EntityType.Id == entityTypeId),
                        Value = Convert.ToDateTime(udfValue).Date
                    };

                case FieldTypeDto.DateTime:
                    return new DateUdfValueDto
                    {
                        EffectiveDate = DateTime.Now,
                        FieldDefinitionClass = fieldDefinition.FieldDefinitionClasses.FirstOrDefault(fdc => fdc.EntityType.Id == entityTypeId),
                        Value = Convert.ToDateTime(udfValue)
                    };

                case FieldTypeDto.Text:
                    return new TextUdfValueDto
                    {
                        EffectiveDate = DateTime.Now,
                        FieldDefinitionClass = fieldDefinition.FieldDefinitionClasses.FirstOrDefault(fdc => fdc.EntityType.Id == entityTypeId),
                        Value = Convert.ToString(udfValue)
                    };

                case FieldTypeDto.Lookup:
                    return new UserDefinedDataSetUdfValueDto
                    {
                        EffectiveDate = DateTime.Now,
                        FieldDefinitionClass = fieldDefinition.FieldDefinitionClasses.FirstOrDefault(fdc => fdc.EntityType.Id == entityTypeId),
                        Value = fieldDefinition.UserDefinedDataSet.Values.FirstOrDefault(v => v.Name == Convert.ToString(udfValue))
                    };

                case FieldTypeDto.SystemLookup:
                    var systemDataSetValues = SdsService.Find(fieldDefinition.SystemDataSet.Id).ToArray();

                    if (!systemDataSetValues.Any())
                        return null;

                    return new SystemDataSetUdfValueDto
                    {
                        EffectiveDate = DateTime.Now,
                        FieldDefinitionClass = fieldDefinition.FieldDefinitionClasses.FirstOrDefault(fdc => fdc.EntityType.Id == entityTypeId),
                        Value = systemDataSetValues.FirstOrDefault(value =>
                            string.Compare(value.Name, Convert.ToString(udfValue), StringComparison.InvariantCultureIgnoreCase) == 0)
                    };

                default:
                    throw new NotSupportedException($"{fieldDefinition.Type}' is not supported");
            }
        }

        public void AddOrUpdateUdf<T>(T t, int entityTypeId, int udfId, string udfValue) where T : IFieldExtensibleDto
        {
            var field = GetFields(entityTypeId).FirstOrDefault(f => f.Id == udfId);

            if (field == null)
                return;

            var existUdf = t.Udfs.FirstOrDefault(u => u.FieldDefinitionClass.Entity.Id == udfId);

            if (existUdf != null)
                UpdateUdf(existUdf, field, udfValue);
            else
                t.Udfs.Add(GetUdf(field, udfValue, entityTypeId));
        }

        private void UpdateUdf(UdfValueDto udfValueDto, FieldDefinitionDto fieldDefinition, object udfValue)
        {
            switch (fieldDefinition.Type)
            {
                case FieldTypeDto.Boolean:
                    (udfValueDto as BooleanUdfValueDto).Value = Convert.ToBoolean(udfValue);
                    break;

                case FieldTypeDto.Currency:
                    (udfValueDto as CurrencyUdfValueDto).Value = Convert.ToDecimal(udfValue);
                    break;

                case FieldTypeDto.Percentage:
                    (udfValueDto as PercentageUdfValueDto).Value = Convert.ToDecimal(udfValue);
                    break;

                case FieldTypeDto.Numeric:
                    (udfValueDto as NumericUdfValueDto).Value = Convert.ToDecimal(udfValue);
                    break;

                case FieldTypeDto.Date:
                    (udfValueDto as DateUdfValueDto).Value = Convert.ToDateTime(udfValue);
                    break;

                case FieldTypeDto.DateTime:
                    (udfValueDto as DateUdfValueDto).Value = Convert.ToDateTime(udfValue);
                    break;

                case FieldTypeDto.Text:
                    (udfValueDto as TextUdfValueDto).Value = Convert.ToString(udfValue);
                    break;

                case FieldTypeDto.Lookup:
                    (udfValueDto as UserDefinedDataSetUdfValueDto).Value =
                        fieldDefinition.UserDefinedDataSet.Values.FirstOrDefault(v => v.Name == Convert.ToString(udfValue));
                    break;

                case FieldTypeDto.SystemLookup:
                    var systemDatasetValues = SdsService.Find(fieldDefinition.SystemDataSet.Id).ToArray();

                    if (systemDatasetValues.Any())
                    {
                        (udfValueDto as SystemDataSetUdfValueDto).Value = systemDatasetValues.FirstOrDefault(value =>
                            string.Compare(value.Name, Convert.ToString(udfValue), StringComparison.InvariantCultureIgnoreCase) == 0);
                    }

                    break;

                default:
                    throw new NotSupportedException($"{fieldDefinition.Type}' is not supported");
            }
        }
    }
}
