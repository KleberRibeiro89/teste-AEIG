import { Button, Group, NumberInput, Paper, Stack, Text, TextInput, Title } from '@mantine/core'
import { useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router'
import { associadoApi } from './associadoCrud.ts'

export function NovoAssociadoPage() {
  const navigate = useNavigate()
  const [name, setName] = useState('')
  const [cpf, setCpf] = useState('')
  const [monthlyFee, setMonthlyFee] = useState<number | string>('')
  const [error, setError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const trimmedName = name.trim()
    const digits = cpf.replace(/\D/g, '')
    const fee = typeof monthlyFee === 'number' ? monthlyFee : Number(String(monthlyFee).replace(',', '.'))

    if (!trimmedName) {
      setError('Informe o nome.')
      return
    }
    if (digits.length !== 11) {
      setError('CPF deve ter 11 dígitos.')
      return
    }
    if (!Number.isFinite(fee) || fee <= 0) {
      setError('Informe o valor da mensalidade.')
      return
    }

    setSaving(true)
    setError(null)
    try {
      await associadoApi.create({ name: trimmedName, cpf: digits, monthlyFee: fee })
      navigate('/associados')
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Não foi possível salvar.')
      setSaving(false)
    }
  }

  return (
    <Stack gap="md" p="xl" maw={560}>
      <Title order={2}>Novo associado</Title>
      <Paper component="form" onSubmit={submit} p="lg" radius="md" bg="dark.6" withBorder>
        <Stack gap="md">
          {error && (
            <Text c="red" size="sm">
              {error}
            </Text>
          )}
          <TextInput label="Nome" value={name} onChange={(event) => setName(event.currentTarget.value)} required />
          <TextInput
            label="CPF"
            value={cpf}
            onChange={(event) => setCpf(event.currentTarget.value)}
            placeholder="000.000.000-00"
            required
          />
          <NumberInput
            label="Mensalidade"
            value={monthlyFee}
            onChange={setMonthlyFee}
            min={0}
            decimalScale={2}
            fixedDecimalScale
            prefix="R$ "
            decimalSeparator=","
            thousandSeparator="."
            required
          />
          <Group justify="flex-end">
            <Button variant="default" onClick={() => navigate('/associados')} disabled={saving}>
              Cancelar
            </Button>
            <Button type="submit" loading={saving}>
              Salvar
            </Button>
          </Group>
        </Stack>
      </Paper>
    </Stack>
  )
}
