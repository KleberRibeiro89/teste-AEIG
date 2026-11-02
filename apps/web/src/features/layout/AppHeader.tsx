import { Button, Group } from '@mantine/core'
import { Link } from 'react-router'
import logo from '../../assets/logo.png'

interface AppHeaderProps {
  onLogout: () => void
}

export function AppHeader({ onLogout }: AppHeaderProps) {
  return (
    <Group h="100%" px="md" justify="space-between">
      <Link to="/" aria-label="Página principal" className="app-logo-link">
        <img src={logo} alt="" className="app-logo" />
      </Link>
      <Button variant="subtle" color="orange" onClick={onLogout}>
        Sair
      </Button>
    </Group>
  )
}
