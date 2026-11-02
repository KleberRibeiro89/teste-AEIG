import { notifications } from '@mantine/notifications'
import { HubConnectionBuilder } from '@microsoft/signalr'
import { useEffect } from 'react'
import { formatDate, money } from '../../lib/format.ts'
import { apiOrigin } from '../../lib/http.ts'

interface CobrancaBaixada {
  nossoNumero: string
  nomeStatus?: string | null
  valorPago: number
  dataPagamento: string
}

export function CobrancaNotifications() {
  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(`${apiOrigin()}/hubs/cobranca`, {
        accessTokenFactory: () => localStorage.getItem('token') ?? '',
      })
      .withAutomaticReconnect()
      .build()

    connection.on('CobrancaBaixada', (aviso: CobrancaBaixada) => {
      notifications.show({
        title: 'Cobrança baixada',
        message: `Nosso número ${aviso.nossoNumero} · ${aviso.nomeStatus ?? 'Pago'} · ${money.format(aviso.valorPago)} em ${formatDate(aviso.dataPagamento)}`,
        color: 'green',
        autoClose: 6000,
      })
    })

    connection.start().catch(() => {})
    return () => {
      connection.stop().catch(() => {})
    }
  }, [])

  return null
}
