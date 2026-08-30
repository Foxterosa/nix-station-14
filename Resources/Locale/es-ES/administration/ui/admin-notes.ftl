# UI
admin-notes-title = Notas de {$player}
admin-notes-new-note = Nota nueva
admin-notes-show-more = Mostrar más
admin-notes-for = Nota para: {$player}
admin-notes-id = ID: { $id }
admin-notes-type = Tipo: {$type}
admin-notes-severity = Gravedad: {$severity}
admin-notes-secret = Secreta
admin-notes-notsecret = No secreta
admin-notes-expires = Vence el: {$expires}
admin-notes-expires-never = No vence
admin-notes-edited-never = Nunca
admin-notes-round-id = Id de ronda: {$id}
admin-notes-round-id-unknown = Id de ronda: desconocido
admin-notes-created-by = Creada por: {$author}
admin-notes-created-at = Creada el: {$date}
admin-notes-last-edited-by = Última edición por: {$author}
admin-notes-last-edited-at = Última edición el: {$date}
admin-notes-edit = Editar
admin-notes-delete = Eliminar
admin-notes-hide = Ocultar
admin-notes-delete-confirm = Confirmar eliminación
admin-notes-edited = Última edición de {$author} el {$date}
admin-notes-unbanned = Desbaneado por {$admin} el {$date}
admin-notes-message-desc = [color=white]Has recibido { $count ->
    [1] un mensaje administrativo
    *[other] mensajes administrativos
} desde la última vez que jugaste en este servidor.[/color]
admin-notes-message-admin = De [bold]{ $admin }[/bold], escrito el { TOSTRING($date, "f") }:
admin-notes-message-wait = El botón de aceptar se habilitará después de {$time} segundos.
admin-notes-message-accept = Descartar permanentemente
admin-notes-message-dismiss = Descartar por ahora
admin-notes-message-seen = Visto
admin-notes-banned-from = Baneado de
admin-notes-the-server = el servidor
admin-notes-permanently = permanentemente
admin-notes-days = {$days} días
admin-notes-hours = {$hours} horas
admin-notes-minutes = {$minutes} minutos

# Note editor UI
admin-note-editor-title-new = Creando una nota nueva para {$player}
admin-note-editor-title-existing = Editando la nota {$id} de {$player} hecha por {$author}
admin-note-editor-pop-out = Desacoplar
admin-note-editor-secret = ¿Secreta?
admin-note-editor-secret-tooltip = Si marcas esto, la nota no será visible para el jugador
admin-note-editor-type-note = Nota
admin-note-editor-type-message = Mensaje
admin-note-editor-type-watchlist = Lista de vigilancia
admin-note-editor-type-server-ban = Baneo del servidor
admin-note-editor-type-role-ban = Baneo de rol
admin-note-editor-severity-select = Seleccionar
admin-note-editor-severity-none = Ninguna
admin-note-editor-severity-minor = Leve
admin-note-editor-severity-low = Baja
admin-note-editor-severity-medium = Media
admin-note-editor-severity-high = Alta
admin-note-editor-expiry-checkbox = ¿Permanente?
admin-note-editor-expiry-checkbox-tooltip = Marca esto para que venza
admin-note-editor-expiry-label = Vence en:
admin-note-editor-expiry-label-params = Vence el: {$date} (en {$expiresIn})
admin-note-editor-expiry-label-expired = Vencida
admin-note-editor-expiry-placeholder = Ingresa el tiempo de vencimiento (número entero).
admin-note-editor-submit = Enviar
admin-note-editor-submit-confirm = ¿Estás seguro?

# Time
admin-note-button-minutes = Minutos
admin-note-button-hours = Horas
admin-note-button-days = Días
admin-note-button-weeks = Semanas
admin-note-button-months = Meses
admin-note-button-years = Años
admin-note-button-centuries = Siglos


# Verb
admin-notes-verb-text = Abrir notas de admin

# Watchlist and message login
admin-notes-watchlist = Lista de vigilancia de {$player}: {$message}
admin-notes-new-message = Recibiste un mensaje de admin de {$admin}: {$message}
admin-notes-fallback-admin-name = [Sistema]

# Admin remarks
admin-remarks-command-description = Abre la página de observaciones de admin
admin-remarks-command-error = Las observaciones de admin están deshabilitadas
admin-remarks-title = Observaciones de admin

# Misc
system-user = [Sistema]
