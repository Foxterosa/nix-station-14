lathe-menu-title = Menú del fabricador
lathe-menu-queue = Cola
lathe-menu-server-list = Lista de servidores
lathe-menu-sync = Sincronizar
lathe-menu-search-designs = Buscar diseños
lathe-menu-category-all = Todos
lathe-menu-search-filter = Filtro:
lathe-menu-amount = Cantidad:
lathe-menu-recipe-count = { $count ->
    [1] {$count} receta
    *[other] {$count} recetas
}
lathe-menu-reagent-slot-examine = Tiene una ranura para un vaso de precipitados en el costado.
lathe-reagent-dispense-no-container = ¡El líquido se derrama de {THE($name)} al suelo!
lathe-menu-result-reagent-display = {$reagent} ({$amount}u)
lathe-menu-material-display = {$material} ({$amount})
lathe-menu-tooltip-display = {$amount} de {$material}
lathe-menu-description-display = [italic]{$description}[/italic]
lathe-menu-material-amount = { $amount ->
    [1] {NATURALFIXED($amount, 2)} {$unit}
    *[other] {NATURALFIXED($amount, 2)} {$unit}
}
lathe-menu-material-amount-missing = { $amount ->
    [1] {NATURALFIXED($amount, 2)} {$unit} de {$material} ([color=red]faltan {NATURALFIXED($missingAmount, 2)} {$unit}[/color])
    *[other] {NATURALFIXED($amount, 2)} {$unit} de {$material} ([color=red]faltan {NATURALFIXED($missingAmount, 2)} {$unit}[/color])
}
lathe-menu-no-materials-message = No hay materiales cargados.
lathe-menu-silo-linked-message = Silo enlazado
lathe-menu-fabricating-message = Fabricando...
lathe-menu-materials-title = Materiales
lathe-menu-queue-title = Cola de fabricación
lathe-menu-delete-fabricating-tooltip = Cancelar la impresión del objeto actual.
lathe-menu-delete-item-tooltip = Cancelar la impresión de este lote.
lathe-menu-move-up-tooltip = Adelantar este lote en la cola.
lathe-menu-move-down-tooltip = Retrasar este lote en la cola.
lathe-menu-item-single = {$index}. {$name}
lathe-menu-item-batch = {$index}. {$name} ({$printed}/{$total})
