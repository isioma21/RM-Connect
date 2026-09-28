import { createTheme } from '@mui/material/styles'

const headingFont = "'Fraunces', Georgia, serif"

export const theme = createTheme({
  palette: {
    primary: { main: '#0F5C55', dark: '#0A413C', light: '#E3EFED' },
    error: { main: '#A33A2B' },
    background: { default: '#F4F2EC', paper: '#FFFFFF' },
    text: { primary: '#1B1F24', secondary: '#5B616B' },
    divider: '#E2DED3',
  },
  shape: { borderRadius: 10 },
  typography: {
    fontFamily: "'Plus Jakarta Sans', system-ui, sans-serif",
    h1: { fontFamily: headingFont, fontSize: '2.1rem', fontWeight: 600 },
    h2: { fontFamily: headingFont, fontSize: '1.75rem', fontWeight: 600 },
    h3: { fontFamily: headingFont, fontSize: '1.4rem', fontWeight: 600 },
    button: { textTransform: 'none', fontWeight: 600 },
  },
  components: {
    MuiButton: {
      defaultProps: { disableElevation: true },
      styleOverrides: { root: { minHeight: 44, paddingInline: 18 } },
    },
    MuiCard: {
      defaultProps: { variant: 'outlined' },
      styleOverrides: { root: { borderRadius: 16 } },
    },
  },
})
