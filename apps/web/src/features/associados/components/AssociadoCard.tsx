import { Paper, Text } from '@mantine/core'
import { formatCpf, money } from '../../../lib/format.ts'
import type { Associado } from '../associadoCrud.ts'

interface AssociadoCardProps {
  associado: Associado
  onOpen: () => void
}

export function AssociadoCard({ associado, onOpen }: AssociadoCardProps) {
  return (
    <Paper p="md" radius="md" bg="dark.6" withBorder>
      <Text
        component="button"
        type="button"
        fw={600}
        c="orange"
        onClick={onOpen}
        style={{ background: 'none', border: 0, padding: 0, cursor: 'pointer', textAlign: 'left' }}
      >
        {associado.nome}
      </Text>
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
