using System;
using System.Collections.Generic;
using System.Text;

namespace Imigrei.Domain.Enums;

public enum TipoAcaoImigranteEnum
{
    NovoPedido = 1,
    AtualizacaoDados = 2,
    CancelamentoPedido = 3,
    FinalizacaoPedido = 4,
    Outros = 5
}
