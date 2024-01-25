# Sistema de Gerenciamento de Estacionamentos

Este projeto é um sistema de gerenciamento de estacionamentos desenvolvido em .NET. Ele permite que proprietários de estacionamentos se cadastrem, gerenciem suas vagas e clientes façam reservas.

## Funcionalidades

1. **Autenticação**:
   - Os usuários podem se cadastrar, fazer login e atualizar tokens de acesso através dos endpoints `/api/auth/cadastro`, `/api/auth/login` e `api/auth/refresh-token`.

2. **Clientes**:
   - Os clientes podem reservar vagas em estacionamentos específicos (`POST /reservar/{estacionamentoId}`), visualizar suas reservas (`GET /api/reservas`) e cancelar reservas (`DELETE /api/cancelar-reserva/{vagaId}`).

3. **Estacionamentos**:
   - Todos os estacionamentos podem ser visualizados através do endpoint `GET /api/estacionamentos`.

4. **Proprietários**:
   - Os proprietários podem gerenciar seus estacionamentos através de vários endpoints. Eles podem criar um novo estacionamento (`POST /api`), visualizar detalhes do estacionamento (`GET /api`), atualizar detalhes do estacionamento (`PUT /api/{estacionamentoId}`), criar uma nova vaga (`POST /api/{estacionamentoId}/criar-vaga`), visualizar todas as vagas (`GET /api/{estacionamentoId}/vagas`), excluir uma vaga (`DELETE /api/excluir/{vagaId}`), visualizar todas as reservas (`GET /api/{estacionamentoId}/reservas`) e liberar uma reserva (`DELETE /api/liberar-reserva/{vagaId}`).

## Banco de Dados

O sistema é integrado a um banco de dados SQL Server, garantindo a persistência e a segurança dos dados.

