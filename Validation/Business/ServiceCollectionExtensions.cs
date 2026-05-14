using Pacs008_Validator.Validation.Business.Rules;

namespace Pacs008_Validator.Validation.Business
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessRules(this IServiceCollection services)
        {
            services.AddSingleton<IValidationRule, DemoRule>();
            services.AddSingleton<BusinessRuleEngine>();
            return services;

        }
    }
}