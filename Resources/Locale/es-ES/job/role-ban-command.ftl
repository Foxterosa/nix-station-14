### Localization for role ban command

cmd-roleban-desc = Banea a un jugador de un puesto
cmd-roleban-help = Uso: roleban <nombre o ID de usuario> <puesto> <razón> [duración en minutos, omitir o 0 para baneo permanente]

## Completion result hints
cmd-roleban-hint-1 = <nombre o ID de usuario>
cmd-roleban-hint-2 = <puesto>
cmd-roleban-hint-3 = <razón>
cmd-roleban-hint-4 = [duración en minutos, omitir o 0 para baneo permanente]
cmd-roleban-hint-5 = [severidad]

cmd-roleban-hint-duration-1 = Permanente
cmd-roleban-hint-duration-2 = 1 día
cmd-roleban-hint-duration-3 = 3 días
cmd-roleban-hint-duration-4 = 1 semana
cmd-roleban-hint-duration-5 = 2 semanas
cmd-roleban-hint-duration-6 = 1 mes


### Localization for role unban command

cmd-roleunban-desc = Perdona el baneo de puesto de un jugador
cmd-roleunban-help = Uso: roleunban <id del baneo de puesto>
cmd-roleunban-unable-to-parse-id = No se pudo interpretar {$id} como un id de baneo entero.
                                   {$help}

## Completion result hints
cmd-roleunban-hint-1 = <id del baneo de puesto>


### Localization for roleban list command

cmd-rolebanlist-desc = Lista los baneos de puesto del usuario
cmd-rolebanlist-help = Uso: <nombre o ID de usuario> [incluir desbaneados]

## Completion result hints
cmd-rolebanlist-hint-1 = <nombre o ID de usuario>
cmd-rolebanlist-hint-2 = [incluir desbaneados]


cmd-roleban-minutes-parse = {$time} no es una cantidad válida de minutos.\n{$help}
cmd-roleban-severity-parse = ${severity} no es una severidad válida\n{$help}.
cmd-roleban-arg-count = Cantidad de argumentos inválida.
cmd-roleban-job-parse = El puesto {$job} no existe.
cmd-roleban-name-parse = No se pudo encontrar a ningún jugador con ese nombre.
cmd-roleban-existing = {$target} ya tiene un baneo de puesto para {$role}.
cmd-roleban-success = Se baneó a {$target} del puesto {$role} con la razón {$reason} {$length}.

cmd-roleban-inf = permanentemente
cmd-roleban-until =  hasta {$expires}

# Department bans
cmd-departmentban-desc = Banea a un jugador de los puestos que componen un departamento
cmd-departmentban-help = Uso: departmentban <nombre o ID de usuario> <departamento> <razón> [duración en minutos, omitir o 0 para baneo permanente]
