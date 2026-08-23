### Comandos de consola relacionados con el sistema de votación

## 'createvote' command

cmd-createvote-desc = Crea una votación
cmd-createvote-help = Uso: createvote <'restart'|'preset'|'map'>
cmd-createvote-cannot-call-vote-now = ¡No podés iniciar una votación ahora mismo!
cmd-createvote-invalid-vote-type = Tipo de votación inválido
cmd-createvote-arg-vote-type = <tipo de votación>

## 'customvote' command

cmd-customvote-desc = Crea una votación personalizada
cmd-customvote-help = Uso: customvote <título> <opción1> <opción2> [opción3...]
cmd-customvote-on-finished-tie = La votación '{$title}' terminó: ¡empate entre {$ties}!
cmd-customvote-on-finished-win = La votación '{$title}' terminó: ¡gana {$winner}!
cmd-customvote-arg-title = <título>
cmd-customvote-arg-option-n = <option{ $n }>

## 'vote' command

cmd-vote-desc = Vota en una votación activa
cmd-vote-help = vote <voteId> <opción>
cmd-vote-cannot-call-vote-now = ¡No podés iniciar una votación ahora mismo!
cmd-vote-on-execute-error-must-be-player = Debés ser un jugador
cmd-vote-on-execute-error-invalid-vote-id = ID de votación inválido
cmd-vote-on-execute-error-invalid-vote-options = Opciones de votación inválidas
cmd-vote-on-execute-error-invalid-vote = Votación inválida
cmd-vote-on-execute-error-invalid-option = Opción inválida

## 'listvotes' command

cmd-listvotes-desc = Lista las votaciones activas
cmd-listvotes-help = Uso: listvotes

## 'cancelvote' command

cmd-cancelvote-desc = Cancela una votación activa
cmd-cancelvote-help = Uso: cancelvote <id>
                      Podés obtener el ID con el comando listvotes.
cmd-cancelvote-error-invalid-vote-id = ID de votación inválido
cmd-cancelvote-error-missing-vote-id = Falta el ID
cmd-cancelvote-arg-id = <id>
