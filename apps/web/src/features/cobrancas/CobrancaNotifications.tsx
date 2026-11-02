import { notifications } from '@mantine/notifications'
import { HubConnectionBuilder } from '@microsoft/signalr'
import { useEffect } from 'react'
import { formatDate, money } from '../../lib/format.ts'
import { apiOrigin } from '../../lib/http.ts'
import { unauthorizedMiddleware } from '../../lib/unauthorized.ts'

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

    connection.start().catch((error: unknown) => {
      const statusCode = typeof error === 'object' && error && 'statusCode' in error ? error.statusCode : undefined
      if (statusCode === 401) unauthorizedMiddleware('/hubs/cobranca', 401)
    })
    return () => {
      connection.stop().catch(() => {})
    }
  }, [])

  return null
}
