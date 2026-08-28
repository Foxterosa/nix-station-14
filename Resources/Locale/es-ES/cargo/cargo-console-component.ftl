## UI

cargo-console-menu-title = Consola de Pedido de Carga
cargo-console-menu-flavor-left = Order even more pizza boxes than usual!
cargo-console-menu-flavor-right = v2.1
cargo-console-menu-account-name-label = Nombre de la cuenta: { " " }
cargo-console-menu-account-name-none-text = No
cargo-console-menu-account-name-format = [bold][color={$color}]{$name}[/color][/bold] [font="Monospace"]\[{$code}\][/font]
cargo-console-menu-shuttle-name-label = Nombre del transbordador: { " " }
cargo-console-menu-shuttle-name-none-text = No
cargo-console-menu-points-label = Créditos: { " " }
cargo-console-menu-points-amount = ${ $amount }
cargo-console-menu-shuttle-status-label = Estado del transbordador: { " " }
cargo-console-menu-shuttle-status-away-text = Izquierda
cargo-console-menu-order-capacity-label = Volumen de pedido: { " " }
cargo-console-menu-call-shuttle-button = Activar Telepad
cargo-console-menu-permissions-button = Accesos
cargo-console-menu-categories-label = Categorías: { " " }
cargo-console-menu-search-bar-placeholder = Búsqueda
cargo-console-menu-requests-label = Consultas
cargo-console-menu-orders-label = Pedidos
cargo-console-menu-populate-categories-all-text = Todos
cargo-console-menu-order-row-title = {$productName} (x{$orderAmount} for {$orderPrice}$)
cargo-console-menu-populate-orders-cargo-order-row-product-name-text = Requested by: {$orderRequester} from [color={$accountColor}]{$account}[/color]
cargo-console-menu-order-row-product-description = Reason: {$orderReason}
cargo-console-menu-order-row-button-approve = Approve
cargo-console-menu-order-row-button-cancel = Cancel
cargo-console-menu-order-row-alerts-reason-absent = The reason is not specified
cargo-console-menu-order-row-alerts-requester-unknown = Unknown
cargo-console-menu-tab-title-orders = Orders
cargo-console-menu-tab-title-funds = Transfers
cargo-console-menu-account-action-transfer-limit = [bold]Transfer Limit:[/bold] ${$limit}
cargo-console-menu-account-action-transfer-limit-unlimited-notifier = [color=gold](Unlimited)[/color]
cargo-console-menu-account-action-select = [bold]Account Action:[/bold]
cargo-console-menu-account-action-amount = [bold]Amount:[/bold] $
cargo-console-menu-account-action-button = Transfer
cargo-console-menu-toggle-account-lock-button = Toggle Transfer Limit
cargo-console-menu-account-action-option-withdraw = Withdraw Cash
cargo-console-menu-account-action-option-transfer = Transfer Funds to {$code}

# Orders
cargo-console-order-not-allowed = Acceso denegado
cargo-console-station-not-found = No hay complejo accesible
cargo-console-invalid-product = ID de producto inválido
cargo-console-too-many = Demasiados pedidos aprobados
cargo-console-snip-snip = Orden reducida a su capacidad
cargo-console-insufficient-funds = Fondos insuficientes ({ $cost } necesario)
cargo-console-unfulfilled = No hay lugar para cumplir el pedido
cargo-console-trade-station = Envía a { $destination }
cargo-console-unlock-approved-order-broadcast = [bold]An pedido de { $productName } x{ $orderAmount }[/bold] valor [bold]{ $cost }[/bold] fue aprobado [bold]{ $approver }[/bold]
cargo-console-fund-withdraw-broadcast = [bold]{$name} withdrew {$amount} spesos from {$name1} \[{$code1}\]
cargo-console-fund-transfer-broadcast = [bold]{$name} transferred {$amount} spesos from {$name1} \[{$code1}\] to {$name2} \[{$code2}\][/bold]
cargo-console-fund-transfer-user-unknown = Unknown

cargo-console-paper-reason-default = None
cargo-console-paper-approver-default = Self
cargo-console-paper-print-name = Orden #{ $orderNumber }
cargo-console-paper-print-text = [head=2]Order #{$orderNumber}[/head]
    {"[bold]Item:[/bold]"} {$itemName} (x{$orderQuantity})
    {"[bold]Requested by:[/bold]"} {$requester}

    {"[head=3]Order Information[/head]"}
    {"[bold]Payer[/bold]:"} {$account} [font="Monospace"]\[{$accountcode}\][/font]
    {"[bold]Approved by:[/bold]"} {$approver}
    {"[bold]Reason:[/bold]"} {$reason}

# Cargo shuttle console
cargo-shuttle-console-menu-title = Consola de llamada del transbordador de carga
cargo-shuttle-console-station-unknown = Desconocido
cargo-shuttle-console-shuttle-not-found = No encontrado
cargo-shuttle-console-organics = Formas de vida orgánica descubiertas en el transbordador
cargo-no-shuttle = ¡Transbordador de carga no encontrado!
cargo-funding-alloc-console-menu-title = Funding Allocation Console
cargo-funding-alloc-console-label-account = [bold]Account[/bold]
cargo-funding-alloc-console-label-code = [bold] Code [/bold]
cargo-funding-alloc-console-label-balance = [bold] Balance [/bold]
cargo-funding-alloc-console-label-cut = [bold] Revenue Division (%) [/bold]

cargo-funding-alloc-console-label-primary-cut = Cargo's cut of funds from non-lockbox sources (%):
cargo-funding-alloc-console-label-lockbox-cut = Cargo's cut of funds from lockbox sales (%):

cargo-funding-alloc-console-label-help-non-adjustible = Cargo receives {$percent}% of profits from non-lockbox sales. The rest is split as specified below:
cargo-funding-alloc-console-label-help-adjustible = Remaining funds from non-lockbox sources are distributed as specified below:
cargo-funding-alloc-console-button-save = Save Changes
cargo-funding-alloc-console-label-save-fail = [bold]Revenue Divisions Invalid![/bold] [color=red]({$pos ->
    [1] +
    *[-1] -
}{$val}%)[/color]

# Slip template
cargo-acquisition-slip-body = [head=3]Asset Detail[/head]
    {"[bold]Product:[/bold]"} {$product}
    {"[bold]Description:[/bold]"} {$description}
    {"[bold]Unit cost:[/bold"}] ${$unit}
    {"[bold]Amount:[/bold]"} {$amount}
    {"[bold]Cost:[/bold]"} ${$cost}

    {"[head=3]Purchase Detail[/head]"}
    {"[bold]Orderer:[/bold]"} {$orderer}
    {"[bold]Reason:[/bold]"} {$reason}
