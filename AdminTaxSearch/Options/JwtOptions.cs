namespace AdminTaxSearch.Options
{
    public record JwtOptions
    {
        public string SecretKey { get; init; } = "CHANGE_THIS_TO_A_LONG_SECRET";
        public string Issuer { get; init; } = "DemoORM";
        public string Audience { get; init; } = "DemoORMClients";
        public int ExpireMinutes { get; init; } = 60;
    }

}
