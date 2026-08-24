parse-minutes-fail = No se pudo interpretar '{$minutes}' como minutos
parse-session-fail = No se encontro una sesion para '{$username}'

## Role Timer Commands

# - playtime_addoverall
cmd-playtime_addoverall-desc = Añade los minutos especificados al tiempo total de juego de un jugador
cmd-playtime_addoverall-help = Uso: {$command} <nombre de usuario> <minutos>
cmd-playtime_addoverall-succeed = Se aumento el tiempo total de {$username} a {TOSTRING($time, "dddd\\:hh\\:mm")}
cmd-playtime_addoverall-arg-user = <nombre de usuario>
cmd-playtime_addoverall-arg-minutes = <minutos>
cmd-playtime_addoverall-error-args = Se esperaban exactamente dos argumentos

# - playtime_addrole
cmd-playtime_addrole-desc = Añade los minutos especificados al tiempo de juego en rol de un jugador
cmd-playtime_addrole-help = Uso: {$command} <nombre de usuario> <rol> <minutos>
cmd-playtime_addrole-succeed = Se aumento el tiempo de rol de {$username} / \'{$role}\' a {TOSTRING($time, "dddd\\:hh\\:mm")}
cmd-playtime_addrole-arg-user = <nombre de usuario>
cmd-playtime_addrole-arg-role = <rol>
cmd-playtime_addrole-arg-minutes = <minutos>
cmd-playtime_addrole-error-args = Se esperaban exactamente tres argumentos

# - playtime_getoverall
cmd-playtime_getoverall-desc = Obtiene el tiempo total de juego de un jugador
cmd-playtime_getoverall-help = Uso: {$command} <nombre de usuario>
cmd-playtime_getoverall-success = El tiempo total de {$username} es {TOSTRING($time, "dddd\\:hh\\:mm")}.
cmd-playtime_getoverall-arg-user = <nombre de usuario>
cmd-playtime_getoverall-error-args = Se esperaba exactamente un argumento

# - GetRoleTimer
cmd-playtime_getrole-desc = Obtiene todos o uno de los temporizadores de rol de un jugador
cmd-playtime_getrole-help = Uso: {$command} <nombre de usuario> [rol]
cmd-playtime_getrole-no = No se encontro ningun temporizador de rol
cmd-playtime_getrole-role = Rol: {$role}, Tiempo jugado: {$time}
cmd-playtime_getrole-overall = El tiempo total jugado es {$time}
cmd-playtime_getrole-succeed = El tiempo de juego de {$username} es: {TOSTRING($time, "dddd\\:hh\\:mm")}.
cmd-playtime_getrole-arg-user = <nombre de usuario>
cmd-playtime_getrole-arg-role = <rol|'Total'>
cmd-playtime_getrole-error-args = Se esperaban exactamente uno o dos argumentos

# - playtime_save
cmd-playtime_save-desc = Guarda los tiempos de juego del jugador en la BD
cmd-playtime_save-help = Uso: {$command} <nombre de usuario>
cmd-playtime_save-succeed = Se guardo el tiempo de juego de {$username}
cmd-playtime_save-arg-user = <nombre de usuario>
cmd-playtime_save-error-args = Se esperaba exactamente un argumento

## 'playtime_flush' command'

cmd-playtime_flush-desc = Vuelca los rastreadores activos al almacenamiento del seguimiento de tiempo de juego.
cmd-playtime_flush-help = Uso: {$command} [nombre de usuario]
    Esto solo vuelca al almacenamiento interno, no a la BD de inmediato.
    Si se proporciona un usuario, solo se vuelca ese usuario.

cmd-playtime_flush-error-args = Se esperaban cero o un argumento
cmd-playtime_flush-arg-user = [nombre de usuario]
