### Localization used for the invoke verb command.
# Mostly help + error messages.

invoke-verb-command-description = Invoca un verbo con el nombre indicado sobre una entidad, usando la entidad del jugador
invoke-verb-command-help = invokeverb <uidJugador | "self"> <uidObjetivo> <nombreVerbo | "interaction" | "activation" | "alternative">

invoke-verb-command-invalid-args = invokeverb toma 2 argumentos.

invoke-verb-command-invalid-player-uid = No se pudo interpretar el uid del jugador, o no se pasó "self".
invoke-verb-command-invalid-target-uid = No se pudo interpretar el uid del objetivo.

invoke-verb-command-invalid-player-entity = El uid de jugador proporcionado no corresponde a una entidad válida.
invoke-verb-command-invalid-target-entity = El uid de objetivo proporcionado no corresponde a una entidad válida.

invoke-verb-command-success = Se invocó el verbo '{ $verb }' sobre { $target } usando a { $player } como usuario.

invoke-verb-command-verb-not-found = No se pudo encontrar el verbo { $verb } en { $target }.
