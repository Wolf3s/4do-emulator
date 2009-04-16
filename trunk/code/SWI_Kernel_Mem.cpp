#include "SWI_Kernel.hpp"

#include "Kernel.hpp"
#include "KernelFaker.hpp"
#include "KernelTypes.hpp"
#include "BitMath.h"

#pragma warning (disable : 4100) // Disable damned "unused param" warnings.

typedef uint8 AllocType; enum
{
    ALLOC_ANY = 0,
    ALLOC_DRAM = 1,
    ALLOC_VRAM = 2
};

// Allocation
MemPtr KRN_malloc( int32 size )
{
	return NULL;
}

MemPtr KRN_AllocMem( int32 size, uint32 flags )
{
    DMA* dma = DMA::getInstance();
    Kernel* kernel = Kernel::getInstance();
    //MemListStruct* memfreelists =  kernel->getData()->kb_MemFreeLists
    //TaskStruct*    currentTask  = (TaskStruct*)   dma->getPtr(kernel->getData()->kb_CurrentTask);
    TaskStruct* currentTask = kernel->getData()->kb_CurrentTask.get();
    MemListStruct* memfreelists =  kernel->getData()->kb_MemFreeLists.get();
    
    //////////////////////////////
    // Get allocation type...
    AllocType allocType;
    if( flags & MEMTYPE_VRAM )
    {
        allocType = ALLOC_VRAM;
        
        // TODO: Add VRAM bank selection here.
    }
    else if( flags & MEMTYPE_DRAM )
    {
        allocType = ALLOC_DRAM;
    }
    else
    {
        allocType = ALLOC_ANY;
    }
    
    //////////////////////////////
    // Extra options...
    
    if( flags & MEMTYPE_STARTPAGE )
    {
        // Allocate along 2k boundary
        if( flags & MEMTYPE_SYSTEMPAGESIZE )
        {
            // Allocate along a page boundary
        }
    }
    
    if( flags & MEMTYPE_MYPOOL )
    {
        // TODO: Don't allocate extra pages in this case.
    }
    
    if( flags & MEMTYPE_FROMTOP )
    {
        // TODO: Allocate from the top instead of the bottom.
    }
    
    
    //////////////////////////////
    // Allocate memory from available memory list.
    
    
    
    //////////////////////////////
    if( flags & MEMTYPE_FILL )
    {
        // TODO: Fill memory with value!
    }

	return NULL;
}

MemPtr KRN_AllocMemBlocks( int32 size, uint32 typebits )
{
	return NULL;
}

MemPtr KRN_AllocMemFromMemList( MemListPtr ml, int32 size, uint32 memflags)
{
	return NULL;
}

MemPtr KRN_AllocMemFromMemLists( ListPtr l, int32 size, uint32 memflags)
{
	return NULL;
}

MemListPtr KRN_AllocMemList( const MemPtr p, CharPtr name )
{
	return NULL;
}

// Deletion
void KRN_free( MemPtr p )
{
}

void KRN_FreeMem( MemPtr p, int32 size )
{
}

void KRN_FreeMemList( MemListPtr ml )
{
}

void KRN_FreeMemToMemList( MemListPtr ml, MemPtr p, int32 size )
{
}

void KRN_FreeMemToMemLists( MemListPtr l, MemPtr p, int32 size )
{
}

int32 KRN_ScavengeMem( void )
{
	return NULL;
}

// Memory info
void   KRN_AvailMem(MemInfoPtr minfo, uint32 memflags)
{
}

Err    KRN_ControlMem( MemPtr p, int32 size, int32 cmd, Item task )
{
	return NULL;
}

int32  KRN_GetMemAllocAlignment( uint32 memflags )
{
	return NULL;
}

int32  KRN_GetMemTrackSize( const MemPtr p )
{
	return NULL;
}

uint32 KRN_GetMemType( const MemPtr p )
{
	return NULL;
}

int32  KRN_GetPageSize( uint32 memflags )
{
	return NULL;
}

bool   KRN_IsMemReadable( const MemPtr p, int32 size )
{
	return NULL;
}

bool   KRN_IsMemWritable( const MemPtr p, int32 size )
{
	return NULL;
}

// Memory debugging
Err KRN_DumpMemDebug(const TagArgPtr args)
{
	return NULL;
}

Err KRN_CreateMemDebug(uint32 controlFlags, const TagArgPtr args)
{
	return NULL;
}

Err KRN_DeleteMemDebug(void)
{
	return NULL;
}

Err KRN_SanityCheckMemDebug(const TagArgPtr args)
{
	return NULL;
}

#pragma warning (default : 4100)