import { useState, type FormEvent } from 'react'
import { Box, Button, Card, CardContent, Link, Stack, Tab, Tabs, TextField, Typography } from '@mui/material'
import { Link as RouterLink, Navigate, useNavigate } from 'react-router-dom'
import { authApi } from '../api/auth'
import type { Role } from '../api/types'
import { homePath, useAuth } from '../auth/AuthContext'
import { ErrorAlert } from '../components/ErrorAlert'
import { PasswordField } from '../components/PasswordField'

const emptyForm = { firstName: '', lastName: '', email: '', phone: '', branch: '', password: '' }

export function RegisterPage() {
  const { user, login } = useAuth()
  const navigate = useNavigate()
  const [role, setRole] = useState<Role>('Customer')
  const [form, setForm] = useState(emptyForm)
  const [error, setError] = useState<unknown>(null)
  const [submitting, setSubmitting] = useState(false)

  if (user) return <Navigate to={homePath(user)} replace />

  const isCustomer = role === 'Customer'

  function update(field: keyof typeof emptyForm, value: string) {
    setForm({ ...form, [field]: value })
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setSubmitting(true)
    setError(null)
    try {
      if (isCustomer) {
        await authApi.registerCustomer({
          firstName: form.firstName, lastName: form.lastName, email: form.email, phone: form.phone, password: form.password,
        })
      } else {
        await authApi.registerManager({
          firstName: form.firstName, lastName: form.lastName, workEmail: form.email, branch: form.branch, password: form.password,
        })
      }
      const loggedIn = await login(form.email, form.password)
      navigate(homePath(loggedIn))
    } catch (err) {
      setError(err)
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <Box sx={{ minHeight: '100vh', display: 'grid', placeItems: 'center', p: 3 }}>
      <Stack spacing={2} sx={{ width: '100%', maxWidth: 520, alignItems: 'center' }}>
        <Typography variant="h3" component="div" color="primary">RM Connect</Typography>
        <Card sx={{ width: '100%' }}>
          <CardContent component="form" onSubmit={handleSubmit} sx={{ p: 5 }}>
            <Stack spacing={2.5}>
              <Typography variant="h2" component="h1">Create your account</Typography>

              <Tabs value={role} onChange={(_, value: Role) => setRole(value)} variant="fullWidth">
                <Tab value="Customer" label="Customer" />
                <Tab value="RelationshipManager" label="Relationship Manager" />
              </Tabs>

              <ErrorAlert error={error} />

              <Stack direction="row" spacing={2}>
                <TextField label="First name" value={form.firstName} onChange={(e) => update('firstName', e.target.value)} autoComplete="given-name" required fullWidth />
                <TextField label="Last name" value={form.lastName} onChange={(e) => update('lastName', e.target.value)} autoComplete="family-name" required fullWidth />
              </Stack>
              <TextField
                label={isCustomer ? 'Email' : 'Work email'}
                type="email"
                value={form.email}
                onChange={(e) => update('email', e.target.value)}
                autoComplete="email"
                required
              />
              {isCustomer ? (
                <TextField label="Phone number" type="tel" value={form.phone} onChange={(e) => update('phone', e.target.value)} autoComplete="tel" required />
              ) : (
                <TextField label="Branch" value={form.branch} onChange={(e) => update('branch', e.target.value)} required />
              )}
              <PasswordField
                label="Password"
                value={form.password}
                onChange={(e) => update('password', e.target.value)}
                autoComplete="new-password"
                helperText="At least 8 characters, with upper and lower case letters and a number."
                required
              />
              <Button type="submit" variant="contained" size="large" disabled={submitting}>
                {submitting ? 'Creating account…' : 'Create account'}
              </Button>
              <Typography color="text.secondary" sx={{ textAlign: 'center' }}>
                Already have an account?{' '}
                <Link component={RouterLink} to="/login" sx={{ fontWeight: 600 }}>Log in</Link>
              </Typography>
            </Stack>
          </CardContent>
        </Card>
      </Stack>
    </Box>
  )
}
