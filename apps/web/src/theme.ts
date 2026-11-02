import { createTheme, type MantineColorsTuple } from '@mantine/core'

const orange: MantineColorsTuple = [
  '#fff4e8',
  '#ffe4c7',
  '#ffd0a3',
  '#ffb56e',
  '#ff9a3c',
  '#ff8514',
  '#f97316',
  '#ea580c',
  '#c2410c',
  '#9a3412',
]

const graphite: MantineColorsTuple = [
  '#f4f4f5',
  '#d4d4d8',
  '#a1a1aa',
  '#71717a',
  '#3f3f46',
  '#2a2a2e',
  '#232326',
  '#1a1a1d',
  '#121214',
  '#0c0c0e',
]

export const theme = createTheme({
  primaryColor: 'orange',
  primaryShade: 6,
  colors: {
    orange,
    dark: graphite,
  },
  black: '#1a1a1d',
  white: '#f4f4f5',
  fontFamily: 'system-ui, Segoe UI, Roboto, sans-serif',
  defaultRadius: 'sm',
})
