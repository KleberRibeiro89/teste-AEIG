import {
  Alert,
  Button,
  Center,
  Image,
  Paper,
  PasswordInput,
  Stack,
  Text,
  TextInput,
  Title,
} from '@mantine/core'
import { useState, type FormEvent } from 'react'
import logo from '../../assets/logo.png'
import { login, type LoginResult } from './login.ts'

interface LoginPageProps {
  onSuccess: (result: LoginResult) => void
}

export function LoginPage({ onSuccess }: LoginPageProps) {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const trimmedEmail = email.trim()
    if (!trimmedEmail || !password) {
      setError('Informe e-mail e senha.')
      return
    }

    setLoading(true)
    setError(null)
    try {
      onSuccess(await login(trimmedEmail, password))
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Não foi possível entrar.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <Center mih="100svh" px="md" bg="dark.7">
      <Paper
        component="form"
        onSubmit={submit}
        w={420}
        maw="100%"
        p="xl"
        radius="md"
        bg="dark.6"
        withBorder
      >
        <Stack gap="lg">
          <Stack gap="xs" align="center">
            <Image src={logo} alt="AEIG" h={48} w="auto" fit="contain" />
            <Title order={2}>Entrar</Title>
            <Text c="dimmed" size="sm" ta="center">
              Acesse com seu e-mail e senha.
            </Text>
          </Stack>

          {error && (
            <Alert color="red" variant="light">
              {error}
            </Alert>
          )}

          <TextInput
            label="E-mail"
            type="email"
            autoComplete="email"
            value={email}
            onChange={(event) => setEmail(event.currentTarget.value)}
            required
          />
          <PasswordInput
            label="Senha"
            autoComplete="current-password"
            value={password}
            onChange={(event) => setPassword(event.currentTarget.value)}
            required
          />
          <Button type="submit" fullWidth loading={loading}>
            Entrar
          </Button>
        </Stack>
      </Paper>
    </Center>
  )
}
