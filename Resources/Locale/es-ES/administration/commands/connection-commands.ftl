## Strings for the "grant_connect_bypass" command.

cmd-grant_connect_bypass-desc = Permite temporalmente que un usuario omita los controles de conexión habituales.
cmd-grant_connect_bypass-help = Uso: grant_connect_bypass <usuario> [duración en minutos]
    Otorga temporalmente a un usuario la capacidad de omitir las restricciones de conexión habituales.
    La omisión solo se aplica a este servidor de juego y vence después de (por defecto) 1 hora.
    Podrá unirse sin importar la lista blanca, el búnker de pánico o el límite de jugadores.

cmd-grant_connect_bypass-arg-user = <usuario>
cmd-grant_connect_bypass-arg-duration = [duración en minutos]

cmd-grant_connect_bypass-invalid-args = Se esperaban 1 o 2 argumentos
cmd-grant_connect_bypass-unknown-user = No se pudo encontrar al usuario '{$user}'
cmd-grant_connect_bypass-invalid-duration = Duración inválida '{$duration}'

cmd-grant_connect_bypass-success = Se agregó correctamente la omisión para el usuario '{$user}'
