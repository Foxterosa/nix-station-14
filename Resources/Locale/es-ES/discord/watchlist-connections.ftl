discord-watchlist-connection-header =
    { $players ->
        [one] {$players} player on a watchlist has
        *[other] {$players} players on a watchlist have
    } connected to {$serverName}

discord-watchlist-connection-entry =
    - { $playerName } con el mensaje "{ $message }"{ $expiry ->
        [0] { "" }
       *[other] { " " }(expira <t:{ $expiry }:R>)
    }{ $otherWatchlists ->
        [0] { "" }
        [one] { " " }y { $otherWatchlists } otra lista de vigilancia
       *[other] { " " }y { $otherWatchlists } otras listas de vigilancia
    }
