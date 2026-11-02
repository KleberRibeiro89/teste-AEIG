import { Button, Group, Paper, PasswordInput, Stack, Text, TextInput, Title } from '@mantine/core'
import { useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router'
import { userApi } from './userCrud.ts'

export function NovoUsuarioPage() {
  const navigate = useNavigate()
  const [nome, setNome] = useState('')
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const trimmedNome = nome.trim()
    const trimmedEmail = email.trim()

    if (!trimmedNome) {
      setError('Informe o nome.')
      return
    }
    if (!trimmedEmail) {
      setError('Informe o e-mail.')
      return
    }
    if (!senha) {
      setError('Informe a senha.')
      return
    }

    setSaving(true)
    setError(null)
    try {
      await userApi.create({ nome: trimmedNome, email: trimmedEmail, senha })
      navigate('/usuarios')
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Não foi possível salvar.')
      setSaving(false)
    }
  }

  return (
    <Stack gap="md" p="xl" maw={560}>
      <Title order={2}>Novo usuário</Title>
      <Paper component="form" onSubmit={submit} p="lg" radius="md" bg="dark.6" withBorder>
        <Stack gap="md">
          {error && (
            <Text c="red" size="sm">
              {error}
            </Text>
          )}
          <TextInput label="Nome" value={nome} onChange={(event) => setNome(event.currentTarget.value)} required />
          <TextInput
            label="E-mail"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.currentTarget.value)}
            required
          />
          <PasswordInput label="Senha" value={senha} onChange={(event) => setSenha(event.currentTarget.value)} required />
          <Group justify="flex-end">
            <Button variant="default" onClick={() => navigate('/usuarios')} disabled={saving}>
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
