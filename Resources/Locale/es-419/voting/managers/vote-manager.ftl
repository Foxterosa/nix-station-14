# Displayed as initiator of vote when no user creates the vote
ui-vote-initiator-server = El servidor

## Default.Votes

ui-vote-restart-title = Reiniciar ronda
ui-vote-restart-succeeded = La votación para reiniciar tuvo éxito.
ui-vote-restart-failed = La votación para reiniciar falló (se necesita { TOSTRING($ratio, "P0") }).
ui-vote-restart-fail-not-enough-ghost-players = La votación para reiniciar falló: se requiere un mínimo de { $ghostPlayerRequirement }% de jugadores fantasma para iniciarla. Actualmente no hay suficientes jugadores fantasma.
ui-vote-restart-yes = Sí
ui-vote-restart-no = No
ui-vote-restart-abstain = Abstenerse

ui-vote-gamemode-title = Siguiente modo de juego
ui-vote-gamemode-tie = ¡Empate en la votación del preset de juego! Se selecciona uno al azar...
ui-vote-gamemode-win = ¡Terminó la votación del preset de juego!

ui-vote-map-title = Siguiente mapa
ui-vote-map-tie = ¡Empate en la votación del mapa! Se selecciona uno al azar...
ui-vote-map-win = ¡Terminó la votación del mapa!
ui-vote-map-notlobby = ¡La votación de mapas solo es válida en el lobby previo a la ronda!
ui-vote-map-notlobby-time = ¡La votación de mapas solo es válida en el lobby previo a la ronda con { $time } restantes!


# Votekick votes
ui-vote-votekick-unknown-initiator = Un jugador
ui-vote-votekick-unknown-target = Jugador desconocido
ui-vote-votekick-title = { $initiator } inició un votekick para el usuario: { $targetEntity }. Motivo: { $reason }
ui-vote-votekick-yes = Sí
ui-vote-votekick-no = No
ui-vote-votekick-abstain = Abstenerse
ui-vote-votekick-success = El votekick para { $target } tuvo éxito. Motivo: { $reason }
ui-vote-votekick-failure = El votekick para { $target } falló. Motivo: { $reason }
ui-vote-votekick-not-enough-eligible = No hay suficientes votantes elegibles conectados para iniciar un votekick: { $voters }/{ $requirement }
ui-vote-votekick-server-cancelled = El servidor canceló el votekick para { $target }.
