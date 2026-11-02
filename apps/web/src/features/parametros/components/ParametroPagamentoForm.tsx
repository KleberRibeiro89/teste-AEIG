import { Button, Group, NumberInput, Paper, Stack, Text } from '@mantine/core'
import { type FormEvent } from 'react'

interface ParametroPagamentoFormProps {
  multa: number | string
  juros: number | string
  error: string | null
  saved: boolean
  saving: boolean
  onMultaChange: (value: number | string) => void
  onJurosChange: (value: number | string) => void
  onSubmit: (event: FormEvent<HTMLFormElement>) => void
}

export function ParametroPagamentoForm({
  multa,
  juros,
  error,
  saved,
  saving,
  onMultaChange,
  onJurosChange,
  onSubmit,
}: ParametroPagamentoFormProps) {
  return (
    <Paper component="form" onSubmit={onSubmit} p="lg" radius="md" bg="dark.6" withBorder>
      <Stack gap="md">
        {error && (
          <Text c="red" size="sm">
            {error}
          </Text>
        )}
        {saved && (
          <Text c="orange" size="sm">
            Parâmetros salvos.
          </Text>
        )}
        <NumberInput
          label="Multa"
          description="Percentual aplicado uma vez após o vencimento"
          value={multa}
          onChange={onMultaChange}
          min={0}
          decimalScale={6}
          suffix=" %"
          decimalSeparator=","
          thousandSeparator="."
          required
        />
        <NumberInput
          label="Juros diários"
          description="Percentual simples por dia corrido de atraso"
          value={juros}
          onChange={onJurosChange}
          min={0}
          decimalScale={6}
          suffix=" %"
          decimalSeparator=","
          thousandSeparator="."
          required
        />
        <Group justify="flex-end">
          <Button type="submit" loading={saving}>
            Salvar
          </Button>
        </Group>
      </Stack>
    </Paper>
  )
}
