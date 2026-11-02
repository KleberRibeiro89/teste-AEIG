import { Paper, Text } from '@mantine/core'
import { formatCpf, money } from '../../../lib/format.ts'
import type { Associado } from '../associadoCrud.ts'

export function AssociadoResumo({ associado }: { associado: Associado }) {
  return (
    <Paper p="md" radius="md" bg="dark.6" withBorder>
      <Text fw={600}>{associado.nome}</Text>
      <Text size="sm" c="dimmed">
        CPF {formatCpf(associado.cpf)}
      </Text>
      <Text size="sm" c="orange">
        Mensalidade {money.format(associado.valorMensalidade)}
      </Text>
      <Text size="sm" c="dimmed">
        Vencimento dia {associado.diaVencimento}
      </Text>
    </Paper>
  )
}
