administration-ui-events-tab-loading = Cargando eventos de estación...
administration-ui-events-tab-search-label = Buscar
administration-ui-events-tab-filter-placeholder = Filtrar eventos
administration-ui-events-tab-sort-label = Ordenar
administration-ui-events-tab-sort-name = Nombre
administration-ui-events-tab-sort-state = Estado
administration-ui-events-tab-sort-availability = Disponibilidad
administration-ui-events-tab-sort-duration = Duración
administration-ui-events-tab-sort-weight = Peso
administration-ui-events-tab-sort-min-players = Jugadores mínimos
administration-ui-events-tab-sort-start = Inicio más temprano
administration-ui-events-tab-sort-cooldown = Enfriamiento
administration-ui-events-tab-sort-ascending = Ascendente
administration-ui-events-tab-sort-descending = Descendente
administration-ui-events-tab-available-only = Solo disponibles
administration-ui-events-tab-refresh = Refrescar
administration-ui-events-tab-force = Forzar
administration-ui-events-tab-force-disabled = No tienes permisos para ejecutar reglas de juego.
administration-ui-events-tab-queue-title = Próximos eventos
administration-ui-events-tab-queue-no-scheduler = No hay un programador compatible activo en esta ronda.
administration-ui-events-tab-queue-empty = La cola de eventos está vacía.
administration-ui-events-tab-queue-count = { $count ->
    [one] { $count } evento en cola.
   *[other] { $count } eventos en cola.
}
administration-ui-events-tab-queue-automatic = AUTO: {$event} ({$id})
administration-ui-events-tab-queue-manual = PROGRAMADO: {$event} ({$id})
administration-ui-events-tab-queue-starts-in = Inicia en {$time}
administration-ui-events-tab-queue-minus-5 = -5m
administration-ui-events-tab-queue-minus-1 = -1m
administration-ui-events-tab-queue-plus-1 = +1m
administration-ui-events-tab-queue-plus-5 = +5m
administration-ui-events-tab-queue-now = Ahora
administration-ui-events-tab-queue-cancel = Cancelar
administration-ui-events-tab-active-title = Eventos activos
administration-ui-events-tab-active-empty = No hay eventos de estación activos actualmente.
administration-ui-events-tab-active-count = { $count ->
    [one] { $count } evento de estación activo.
   *[other] { $count } eventos de estación activos.
}
administration-ui-events-tab-active-remaining = {$remaining} restante de {$duration}
administration-ui-events-tab-active-open = Duración variable | transcurrido {$elapsed}
administration-ui-events-tab-active-end = Finalizar
administration-ui-events-tab-catalog-title = Catálogo de eventos
administration-ui-events-tab-schedule = Programar
administration-ui-events-tab-schedule-minutes-placeholder = min
administration-ui-events-tab-schedule-no-scheduler = No hay un programador compatible activo.
administration-ui-events-tab-enabled = activado
administration-ui-events-tab-disabled = desactivado
administration-ui-events-tab-summary = Eventos: {$count} | Activos: {$active} | Pendientes: {$pending} | Jugadores: {$players} | Ronda: {$minutes} min | Programador: {$enabled}
administration-ui-events-tab-status-available = Auto: disponible
administration-ui-events-tab-status-unavailable = Auto: no disponible
administration-ui-events-tab-meta = Jugadores mín. {$players} | Inicio {$start}m | Enfriamiento {$cooldown}m | Peso {$weight}
administration-ui-events-tab-runtime-idle = Estado: inactivo
administration-ui-events-tab-runtime-pending = Estado: pendiente x{$count}
administration-ui-events-tab-runtime-active = Estado: activo x{$count}
administration-ui-events-tab-runtime-next-start = Inicia en {$time}
administration-ui-events-tab-runtime-remaining = Restante {$time}
administration-ui-events-tab-runtime-remaining-range = Restante {$min}-{$max}
administration-ui-events-tab-runtime-duration = Duración {$duration}
administration-ui-events-tab-duration-open = variable
administration-ui-events-tab-duration-range = {$min}-{$max}
administration-ui-events-tab-queue-scheduler = Programador: { $scheduler }
administration-ui-events-tab-queue-incomplete = { $count ->
    [one] Incompleto: { $count } programador activo no expone cola, por lo que algunos eventos no aparecen aquí.
   *[other] Incompleto: { $count } programadores activos no exponen cola, por lo que algunos eventos no aparecen aquí.
}
administration-ui-events-tab-schedule-minutes-tooltip = Retraso en minutos antes de que ocurra el evento
administration-ui-events-tab-schedule-minus-tooltip = Un minuto antes
administration-ui-events-tab-schedule-plus-tooltip = Un minuto después
administration-ui-events-tab-collapse-tooltip = Colapsar o expandir esta sección
administration-ui-events-tab-meta-occurrences = { $count ->
    [one] Ocurrió { $count } vez en esta ronda
   *[other] Ocurrió { $count } veces en esta ronda
}
