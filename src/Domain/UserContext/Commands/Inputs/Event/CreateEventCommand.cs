using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class CreateEventCommand : Command<CreateEventCommandResult>
{
    public decimal Value { get; set; }
    public char Type { get; set; }
    public string Description { get; set; }
    public DateTime InitialDate { get; set; }
    public DateTime FinalDate { get; set; }
    public string Period { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsTrue(Value >= 0, nameof(Value), "Valor precisa ser maior que zero")
            .IsNotNullOrEmpty(Type.ToString(), nameof(Type), "Tipo do evento é obrigatório")
            .IsNotNullOrEmpty(Description, nameof(Description), "Descrição do evento é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Description, 50, nameof(Description), "Descrição do evento não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(InitialDate.ToString(), nameof(InitialDate), "Data inicial do evento é obrigatório")
            .IsNotNullOrEmpty(FinalDate.ToString(), nameof(FinalDate), "Data Final do evento é obrigatório")
            .IsTrue(InitialDate <= FinalDate, nameof(FinalDate), "Data inicial deve ser menor ou igual a data final")
            .IsNotNullOrEmpty(Period.ToString(), nameof(Period), "Período é obrigatório")
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório")
            .IsNotNullOrEmpty(CategoryId.ToString(), nameof(CategoryId), "Categoria do evento é obrigatório"));

        return Valid;
    }
}