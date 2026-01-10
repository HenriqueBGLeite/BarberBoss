using BarberBoss.Communication.Enums;
using BarberBoss.Communication.Requests;
using Bogus;

namespace CommonTestsUtilities.Requests;

public class RequestUpdateExpenseJsonBuilder
{
    public static RequestUpdateExpenseJson Build()
    {
        List<string> _barbers = ["Barbearia Araujo", "Warley Cabeleleiro"];
        List<string> _services = ["Corte", "Barba", "Corte + Barba", "Sombrancelha"];

        return new Faker<RequestUpdateExpenseJson>()
            .RuleFor(r => r.Date, faker => faker.Date.Past())
            .RuleFor(r => r.BarberName, faker => faker.PickRandom(_barbers))
            .RuleFor(r => r.ServiceName, faker => faker.PickRandom(_services))
            .RuleFor(r => r.ClientName, faker => faker.Person.FullName)
            .RuleFor(r => r.Amount, faker => faker.Random.Decimal(min: 1, max: 1000))
            .RuleFor(r => r.PaymentMethod, faker => faker.PickRandom<PaymentMethod>())
            .RuleFor(r => r.Status, faker => faker.PickRandom<PaymentStatus>())
            .RuleFor(r => r.Notes, faker => faker.Random.Bool(0.3f) ? faker.Lorem.Sentence(3) : null);
    }
}
