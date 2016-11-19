using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace AllReady.UnitTest.Extensions
{
    public class PropertyValidationHelper<TModel>
    {
        private readonly TModel _model;

        public PropertyValidationHelper(TModel model)
        {
            _model = model;
        }

        public IList<ValidationResult> ValidateProperty<TProperty>(Expression<Func<TModel, TProperty>> propertyToValidate)
        {
            var member = GetMemberInfo(propertyToValidate);

            var ctx = new ValidationContext(_model, null, null)
            {
                MemberName = member.Name
            };

            var propertyValue = propertyToValidate.Compile()(_model);

            var validationResults = new List<ValidationResult>();
            Validator.TryValidateProperty(propertyValue, ctx, validationResults);
            return validationResults;
        }

        private MemberInfo GetMemberInfo<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member != null)
                return member.Member;

            throw new ArgumentException("Expression is not a member access", nameof(expression));
        }
    }
}
