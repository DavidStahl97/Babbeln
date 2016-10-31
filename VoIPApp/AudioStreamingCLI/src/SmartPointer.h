#ifndef SMARTPOINTER_H
#define SMARTPOINTER_H

template <class T>
public ref class SmartPointer
{
public:
	SmartPointer(T* t)
		: m_NativePtr(t)
	{		
	}

	SmartPointer()
		: m_NativePtr(nullptr)
	{
	}

	SmartPointer(const SmartPointer<T>% rhs)
	{
		m_NativePtr = new T(*rhs.m_NativePtr);
	}

	~SmartPointer()
	{
		reset();
	}

	void reset()
	{
		delete m_NativePtr;
		m_NativePtr = nullptr;
	}

	T* ptr()
	{
		return m_NativePtr;
	}

	T* operator->()
	{
		return m_NativePtr;
	}

	operator bool()
	{
		return m_NativePtr;
	}

	void operator=(const SmartPointer<T>% rhs)
	{
		*m_NativePtr = *rhs.m_NativePtr;
	}

	void operator=(const T% rhs)
	{
		*m_NativePtr = rhs;
	}

protected:
	!SmartPointer()
	{
		reset();
	}

private:
	T* m_NativePtr;
};

#endif