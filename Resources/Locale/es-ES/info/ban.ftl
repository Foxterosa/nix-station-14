# ban
cmd-ban-desc = Banea a alguien
cmd-ban-help = Uso: ban <nombre o ID de usuario> <motivo> [duración en minutos, omitir o poner 0 para baneo permanente]
cmd-ban-player = No se pudo encontrar a ningún jugador con ese nombre.
cmd-ban-invalid-minutes = ¡{$minutes} no es una cantidad válida de minutos!
cmd-ban-invalid-severity = ¡{$severity} no es una gravedad válida!
cmd-ban-invalid-arguments = Cantidad de argumentos inválida
cmd-ban-hint = <nombre/ID de usuario>
cmd-ban-hint-reason = <motivo>
cmd-ban-hint-duration = [duración]
cmd-ban-hint-severity = [gravedad]

cmd-ban-hint-duration-1 = Permanente
cmd-ban-hint-duration-2 = 1 día
cmd-ban-hint-duration-3 = 3 días
cmd-ban-hint-duration-4 = 1 semana
cmd-ban-hint-duration-5 = 2 semanas
cmd-ban-hint-duration-6 = 1 mes

# ban panel
cmd-banpanel-desc = Abre el panel de baneos
cmd-banpanel-help = Uso: banpanel [nombre o guid de usuario]
cmd-banpanel-server = Esto no se puede usar desde la consola del servidor
cmd-banpanel-player-err = No se pudo encontrar al jugador indicado

# listbans
cmd-banlist-desc = Lista los baneos activos de un usuario.
cmd-banlist-help = Uso: banlist <nombre o ID de usuario>
cmd-banlist-empty = No se encontraron baneos activos para {$user}
cmd-banlist-hint = <nombre/ID de usuario>

cmd-ban_exemption_update-desc = Establece una exención a un tipo de baneo para un jugador.
cmd-ban_exemption_update-help = Uso: ban_exemption_update <jugador> <flag> [<flag> [...]]
    Especifica varios flags para darle a un jugador varias exenciones de baneo.
    Para quitar todas las exenciones, ejecuta este comando y pon "None" como único flag.

cmd-ban_exemption_update-nargs = Se esperaban al menos 2 argumentos
cmd-ban_exemption_update-locate = No se pudo encontrar al jugador '{$player}'.
cmd-ban_exemption_update-invalid-flag = Flag inválido: '{$flag}'.
cmd-ban_exemption_update-success = Se actualizaron los flags de exención de baneo de '{$player}' ({$uid}).
cmd-ban_exemption_update-arg-player = <jugador>
cmd-ban_exemption_update-arg-flag = <flag>

cmd-ban_exemption_get-desc = Muestra las exenciones de baneo de un jugador.
cmd-ban_exemption_get-help = Uso: ban_exemption_get <jugador>

cmd-ban_exemption_get-nargs = Se esperaba exactamente 1 argumento
cmd-ban_exemption_get-none = El usuario no está exento de ningún baneo.
cmd-ban_exemption_get-show = El usuario está exento de los siguientes flags de baneo: {$flags}.
cmd-ban_exemption_get-arg-player = <jugador>

# Ban panel
ban-panel-title = Panel de baneos
ban-panel-player = Jugador
ban-panel-ip = IP
ban-panel-hwid = HWID
ban-panel-reason = Motivo
ban-panel-last-conn = ¿Usar la IP y el HWID de la última conexión?
ban-panel-submit = Banear
ban-panel-confirm = ¿Estás seguro?
ban-panel-tabs-basic = Información básica
ban-panel-tabs-reason = Motivo
ban-panel-tabs-players = Lista de jugadores
ban-panel-tabs-role = Datos del baneo de rol
ban-panel-no-data = Debes indicar un usuario, una IP o un HWID para banear
ban-panel-invalid-ip = No se pudo interpretar la dirección IP. Inténtalo de nuevo
ban-panel-select = Seleccionar tipo
ban-panel-server = Baneo de servidor
ban-panel-role = Baneo de rol
ban-panel-minutes = Minutos
ban-panel-hours = Horas
ban-panel-days = Días
ban-panel-weeks = Semanas
ban-panel-months = Meses
ban-panel-years = Años
ban-panel-permanent = Permanente
ban-panel-ip-hwid-tooltip = Déjalo vacío y marca la casilla de abajo para usar los datos de la última conexión
ban-panel-severity = Gravedad:
ban-panel-erase = Borrar los mensajes de chat y al jugador de la ronda
ban-panel-expiry-error = err

# Ban string
server-ban-string = {$admin} creó un baneo de servidor de gravedad {$severity} que expira {$expires} para [{$name}, {$ip}, {$hwid}], con el motivo: {$reason}
server-ban-string-no-pii = {$admin} creó un baneo de servidor de gravedad {$severity} que expira {$expires} para {$name} con el motivo: {$reason}
server-ban-string-never = nunca

# Kick on ban
ban-kick-reason = Has sido baneado
