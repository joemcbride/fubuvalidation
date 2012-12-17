using System;
using System.Collections.Generic;
using FubuCore.Util;
using FubuValidation.Fields;

namespace FubuValidation
{
    public class ValidationGraph
    {
        private readonly IList<IValidationSource> _sources = new List<IValidationSource>();
        private readonly IFieldValidationQuery _fieldQuery;
        private readonly Cache<Type, ValidationPlan> _plans; 

        public ValidationGraph(IFieldRulesRegistry fieldRegistry, IEnumerable<IValidationSource> sources)
        {
            _fieldQuery = new FieldValidationQuery(fieldRegistry);

            _sources.Fill(new FieldRuleSource(fieldRegistry));
            _sources.Fill(new SelfValidatingClassRuleSource());
            _sources.Fill(sources);

            _plans = new Cache<Type, ValidationPlan>(type => ValidationPlan.For(type, this));
        }

        public IEnumerable<IValidationSource> Sources
        {
            get { return _sources; }
        }

        public IFieldValidationQuery Fields
        {
            get { return _fieldQuery; }
        }

        public void RegisterSource(IValidationSource source)
        {
            _sources.Fill(source);
        }

        public ValidationPlan PlanFor(Type type)
        {
            return _plans[type];
        }

        public static ValidationGraph BasicGraph()
        {
            return new ValidationGraph(FieldRulesRegistry.BasicRegistry(), new IValidationSource[0]);
        }

        // Useful for testing
        public static ValidationGraph For(IValidationSource source)
        {
            return new ValidationGraph(FieldRulesRegistry.BasicRegistry(), new[] { source });
        }
    }
}