import { Avatar, Group, NavLink, Text } from '@mantine/core'
import { type ReactNode } from 'react'
import { AssociadoIcon, ParametroIcon, UsersIcon } from '../../components/icons.tsx'

const menu: { id: string; label: string; path: string; icon: () => ReactNode }[] = [
  { id: 'users', label: 'Usuários', path: '/usuarios', icon: UsersIcon },
  { id: 'associados', label: 'Associados', path: '/associados', icon: AssociadoIcon },
  { id: 'parametros', label: 'Parametrização', path: '/parametros', icon: ParametroIcon },
]

interface AppNavbarProps {
  pathname: string
  iconOnly: boolean
  userName: string
  onNavigate: (path: string) => void
}

export function AppNavbar({ pathname, iconOnly, userName, onNavigate }: AppNavbarProps) {
  return (
    <>
      {menu.map((item) => {
        const Icon = item.icon
        return (
          <NavLink
            key={item.id}
            component="button"
            type="button"
            active={pathname === item.path || pathname.startsWith(`${item.path}/`)}
            label={iconOnly ? undefined : item.label}
            aria-label={item.label}
            leftSection={<Icon />}
            className={iconOnly ? 'app-navlink-icon' : undefined}
            onClick={() => onNavigate(item.path)}
          />
        )
      })}
      <div className="app-navbar-user">
        <hr className="app-navbar-rule" />
        <Group gap="sm" wrap="nowrap" justify={iconOnly ? 'center' : 'flex-start'} title={userName}>
          <Avatar color="orange" radius="xl" size={iconOnly ? 'sm' : 'md'} alt={userName}>
            {initials(userName)}
          </Avatar>
          {!iconOnly && (
            <Text size="sm" fw={600} truncate="end">
              {userName}
            </Text>
          )}
        </Group>
      </div>
    </>
  )
}

function initials(name: string) {
  const parts = name.trim().split(/\s+/).filter(Boolean)
  if (parts.length === 0) return '?'
  if (parts.length === 1) return parts[0].slice(0, 1).toUpperCase()
  return `${parts[0][0]}${parts[parts.length - 1][0]}`.toUpperCase()
}
