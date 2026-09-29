import { useState, type FormEvent } from 'react'
import { Alert, Box, Button, Card, CardContent, Link, Stack, TextField, Typography } from '@mui/material'
import CheckIcon from '@mui/icons-material/Check'
import { Link as RouterLink, Navigate, useLocation, useNavigate, useSearchParams } from 'react-router-dom'
import { homePath, useAuth } from '../auth/AuthContext'
import { ErrorAlert } from '../components/ErrorAlert'
import { PasswordField } from '../components/PasswordField'

const features = [
  'Request the manager you want to work with',
  'Book a call or a branch visit on weekdays',
  'See and sign out the devices you use',
]

export function LoginPage() {
  const { user, login } = useAuth()
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const registeredEmail: string | undefined = useLocation().state?.registeredEmail
  const [email, setEmail] = useState(registeredEmail ?? '')
  const [password, setPassword] = useState('')
  const [error, setError] = useState<unknown>(null)
  const [submitting, setSubmitting] = useState(false)

  if (user) return <Navigate to={homePath(user)} replace />

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setSubmitting(true)
    setError(null)
    try {
      const loggedIn = await login(email, password)
      navigate(homePath(loggedIn))
    } catch (err) {
      setError(err)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex' }}>
      <Box
        sx={{
          width: 520,
          p: 8,
          bgcolor: 'primary.main',
          color: '#FFFFFF',
          display: { xs: 'none', md: 'flex' },
          flexDirection: 'column',
        }}
      >
        <Typography variant="h3" component="div" sx={{ fontSize: '2rem', fontWeight: 700 }}>RM Connect</Typography>
        <Stack spacing={3} sx={{ my: 'auto' }}>
          <Typography variant="h1" sx={{ fontSize: '2.6rem', fontWeight: 500 }}>
            Your relationship manager, one request away.
          </Typography>
          <Stack spacing={1.5} sx={{ color: '#D7E8E5' }}>
            {features.map((feature) => (
              <Stack key={feature} direction="row" spacing={1.5} sx={{ alignItems: 'center' }}>
                <CheckIcon sx={{ color: '#9FD3CB' }} />
                <Typography>{feature}</Typography>
              </Stack>
            ))}
          </Stack>
        </Stack>
      </Box>

      <Box sx={{ flexGrow: 1, display: 'grid', placeItems: 'center', p: 3 }}>
        <Card sx={{ width: '100%', maxWidth: 420 }}>
          <CardContent component="form" onSubmit={handleSubmit} sx={{ p: 5 }}>
            <Stack spacing={2.5}>
              <Box>
                <Typography variant="h2" component="h1">Log in</Typography>
                <Typography color="text.secondary">Use the email you registered with.</Typography>
              </Box>
              {searchParams.has('signedOut') && !error && <Alert severity="info">You have been signed out. Please log in again.</Alert>}
              <ErrorAlert error={error} />
              <TextField label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} autoComplete="email" required autoFocus />
              <PasswordField label="Password" value={password} onChange={(e) => setPassword(e.target.value)} autoComplete="current-password" required />
              <Button type="submit" variant="contained" size="large" disabled={submitting}>
                {submitting ? 'Logging in…' : 'Log in'}
              </Button>
              <Typography color="text.secondary" sx={{ textAlign: 'center' }}>
                New to RM Connect?{' '}
                <Link component={RouterLink} to="/register" sx={{ fontWeight: 600 }}>Create an account</Link>
              </Typography>
            </Stack>
          </CardContent>
        </Card>
      </Box>
    </Box>
  )
}
